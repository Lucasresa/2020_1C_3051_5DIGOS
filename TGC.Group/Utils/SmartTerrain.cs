using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TGC.Core.BoundingVolumes;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Shaders;
using TGC.Core.Textures;

namespace TGC.Group.Utils
{
    public class SmartTerrain
    {
        public SmartTerrain()
        {
            Enabled = true;
            AlphaBlendEnable = false;

            //Shader
            effect = TGCShaders.Instance.VariosShader;
            technique = TGCShaders.T_POSITION_TEXTURED;

            aabb = new TgcBoundingAxisAlignBox();
            random = new Random();
        }

        /// <summary>
        ///     Carga la textura del terreno
        /// </summary>
        public void loadTexture(string path)
        {
            //Dispose textura anterior, si habia
            if (terrainTexture != null && !terrainTexture.Disposed)
            {
                terrainTexture.Dispose();
            }

            var d3dDevice = D3DDevice.Instance.Device;

            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        #region Private fields

        private float maxIntensity;
        private float minIntensity;
        private TGCVector3 traslation;
        private VertexBuffer vbTerrain;
        private CustomVertex.PositionColoredTextured[] vertices;
        private Texture terrainTexture;
        private readonly TgcBoundingAxisAlignBox aabb;
        private Random random;

        #endregion Private fields

        #region Properties

        /// <summary>
        ///     Cantidad de vertices del terrain
        /// </summary>
        public int TotalVertices { get; private set; }

        /// <summary>
        ///     Valor de Y para cada par (X,Z) del Heightmap.
        /// </summary>
        public float[,] HeightmapData { get; private set; }

        /// <summary>
        ///     Indica si la malla esta habilitada para ser renderizada
        /// </summary>
        public bool Enabled { get; set; }

        private TGCVector3 center;

        /// <summary>
        ///     Centro del terreno
        /// </summary>
        public TGCVector3 Center
        {
            get { return center; }
        }

        /// <summary>
        ///     Habilita el renderizado con AlphaBlending para los modelos
        ///     con textura o colores por vértice de canal Alpha.
        ///     Por default está deshabilitado.
        /// </summary>
        public bool AlphaBlendEnable { get; set; }

        protected Effect effect;

        /// <summary>
        ///     Shader del mesh
        /// </summary>
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }
        }

        protected string technique;

        /// <summary>
        ///     Technique que se va a utilizar en el effect.
        ///     Cada vez que se llama a Render() se carga este Technique (pisando lo que el shader ya tenia seteado)
        /// </summary>
        public string Technique
        {
            get { return technique; }
            set { technique = value; }
        }

        public float ScaleXZ { get; set; }

        public float ScaleY { get; set; }

        #endregion Properties

        #region Load heightmap

        /// <summary>
        ///     Carga los valores del Heightmap en una matriz
        /// </summary>
        protected float[,] loadHeightMap(Device d3dDevice, string path)
        {
            var bitmap = (Bitmap)Image.FromFile(path);
            var width = bitmap.Size.Width;
            var length = bitmap.Size.Height;

            var heightmap = new float[length, width];

            for (var i = 0; i < length; i++)
            {
                for (var j = 0; j < width; j++)
                {
                    var pixel = bitmap.GetPixel(j, i);
                    var intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heightmap[i, j] = intensity;
                }
            }
            bitmap.Dispose();
            return heightmap;
        }

        /// <summary>
        ///     Crea la malla de un terreno en base a un Heightmap
        /// </summary>
        /// <param name="heightmapPath">Imagen de Heightmap</param>
        /// <param name="scaleXZ">Escala para los ejes X y Z</param>
        /// <param name="scaleY">Escala para el eje Y</param>
        /// <param name="center">Centro de la malla del terreno</param>
        public void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, TGCVector3 center)
        {
            var d3dDevice = D3DDevice.Instance.Device;
            this.center = center;
            ScaleXZ = scaleXZ;
            ScaleY = scaleY;

            //Dispose de VertexBuffer anterior, si habia
            if (vbTerrain != null && !vbTerrain.Disposed)
            {
                vbTerrain.Dispose();
            }

            //cargar heightmap
            HeightmapData = loadHeightMap(d3dDevice, heightmapPath);

            //Crear vertexBuffer
            TotalVertices = 2 * 3 * (HeightmapData.GetLength(0) - 1) * (HeightmapData.GetLength(1) - 1);
            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionColoredTextured), TotalVertices, d3dDevice,
                Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionColoredTextured.Format, Pool.Default);

            float width = HeightmapData.GetLength(0);
            float length = HeightmapData.GetLength(1);

            traslation.X = center.X - width / 2;
            traslation.Y = center.Y;
            traslation.Z = center.Z - length / 2;

            //Cargar vertices
            loadVertices();
        }

        #endregion Load heightmap

        #region Load & update vertices

        /// <summary>
        ///     Cambia el heightmapData por uno de igual ancho y largo.
        /// </summary>
        /// <param name="heightmapData"></param>
        public void setHeightmapData(float[,] heightmapData)
        {
            if (heightmapData.GetLength(0) == HeightmapData.GetLength(0) &&
                HeightmapData.GetLength(1) == heightmapData.GetLength(1))
            {
                HeightmapData = heightmapData;
            }
        }

        /// <summary>
        ///     Actualiza los vertices segun los valores de HeightmapData
        /// </summary>
        public void updateVertices()
        {
            minIntensity = -1;
            maxIntensity = 0;
            for (var i = 0; i < vertices.Length; i++)
            {
                var v = vertices[i];
                var intensity = HeightmapData[(int)vertices[i].X, (int)vertices[i].Z];
                vertices[i].Y = intensity;
                if (intensity > maxIntensity) maxIntensity = intensity;
                if (minIntensity == -1 || intensity < minIntensity) minIntensity = intensity;
            }

            vbTerrain.SetData(vertices, 0, LockFlags.None);
            aabb.setExtremes(new TGCVector3(0, minIntensity, 0),
                new TGCVector3(HeightmapData.GetLength(0), maxIntensity, HeightmapData.GetLength(1)));
        }

        /// <summary>
        ///     Crea los vertices
        /// </summary>
        private void loadVertices()
        {
            var dataIdx = 0;

            float width = HeightmapData.GetLength(0);
            float length = HeightmapData.GetLength(1);
            var color = Color.White.ToArgb();
            vertices = new CustomVertex.PositionColoredTextured[TotalVertices];

            maxIntensity = 0;
            minIntensity = -1;

            for (var i = 0; i < width - 1; i++)
            {
                for (var j = 0; j < length - 1; j++)
                {
                    if (HeightmapData[i, j] > maxIntensity) maxIntensity = HeightmapData[i, j];
                    if (minIntensity == -1 || HeightmapData[i, j] < minIntensity) minIntensity = HeightmapData[i, j];

                    //Vertices
                    var v1 = new TGCVector3(i, HeightmapData[i, j], j);
                    var v2 = new TGCVector3(i, HeightmapData[i, j + 1], j + 1);
                    var v3 = new TGCVector3(i + 1, HeightmapData[i + 1, j], j);
                    var v4 = new TGCVector3(i + 1, HeightmapData[i + 1, j + 1], j + 1);

                    //Coordendas de textura
                    var t1 = new TGCVector2(i / width, j / length);
                    var t2 = new TGCVector2(i / width, (j + 1) / length);
                    var t3 = new TGCVector2((i + 1) / width, j / length);
                    var t4 = new TGCVector2((i + 1) / width, (j + 1) / length);

                    //Cargar triangulo 1
                    vertices[dataIdx] = new CustomVertex.PositionColoredTextured(v1, color, t1.X, t1.Y);
                    vertices[dataIdx + 1] = new CustomVertex.PositionColoredTextured(v2, color, t2.X, t2.Y);
                    vertices[dataIdx + 2] = new CustomVertex.PositionColoredTextured(v4, color, t4.X, t4.Y);

                    //Cargar triangulo 2
                    vertices[dataIdx + 3] = new CustomVertex.PositionColoredTextured(v1, color, t1.X, t1.Y);
                    vertices[dataIdx + 4] = new CustomVertex.PositionColoredTextured(v4, color, t4.X, t4.Y);
                    vertices[dataIdx + 5] = new CustomVertex.PositionColoredTextured(v3, color, t3.X, t3.Y);

                    dataIdx += 6;
                }
                vbTerrain.SetData(vertices, 0, LockFlags.None);

                aabb.setExtremes(new TGCVector3(0, minIntensity, 0),
                    new TGCVector3(HeightmapData.GetLength(0), maxIntensity, HeightmapData.GetLength(1)));
            }
        }

        #endregion Load & update vertices

        #region Render & Dispose

        /// <summary>
        ///     Renderiza el terreno
        /// </summary>
        public void render()
        {
            if (!Enabled)
                return;

            var d3dDevice = D3DDevice.Instance.Device;
            var texturesManager = TexturesManager.Instance;
            var transform = TGCMatrix.Translation(traslation) * TGCMatrix.Scaling(ScaleXZ, ScaleY, ScaleXZ);

            //Textura
            effect.SetValue("texDiffuseMap", terrainTexture);

            texturesManager.clear(1);

            TGCShaders.Instance.SetShaderMatrix(effect, transform);
            d3dDevice.VertexDeclaration = TGCShaders.Instance.VdecPositionColoredTextured;
            effect.Technique = technique;
            d3dDevice.SetStreamSource(0, vbTerrain, 0);

            //Render con shader
            var p = effect.Begin(0);
            for (var i = 0; i < p; i++)
            {
                effect.BeginPass(i);
                d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, TotalVertices / 3);
                effect.EndPass();
            }
            effect.End();
        }

        /// <summary>
        ///     Libera los recursos del Terreno
        /// </summary>
        public void dispose()
        {
            if (vbTerrain != null)
            {
                vbTerrain.Dispose();
            }

            if (terrainTexture != null)
            {
                terrainTexture.Dispose();
            }
        }

        #endregion Render & Dispose

        #region Utils

        /// <summary>
        ///     Retorna true si hubo interseccion con el terreno y setea el collisionPoint.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="collisionPoint"></param>
        /// <returns></returns>
        public bool intersectRay(TgcRay ray, out TGCVector3 collisionPoint)
        {
            collisionPoint = TGCVector3.Empty;
            var scaleInv = TGCMatrix.Scaling(new TGCVector3(1 / ScaleXZ, 1 / ScaleY, 1 / ScaleXZ));
            var a = TGCVector3.TransformCoordinate(ray.Origin, scaleInv) - traslation;
            var r = TGCVector3.TransformCoordinate(ray.Direction, scaleInv);

            if (a.Y < minIntensity)
                return false;

            TGCVector3 q;
            //Me fijo si intersecta con el BB del terreno.
            if (!TgcCollisionUtils.intersectRayAABB(new TgcRay(a, r).toStruct(), aabb.toStruct(), out q))
                return false;

            float minT = 0;
            //Obtengo el T de la interseccion.
            if (q != a)
            {
                if (r.X != 0) minT = (q.X - a.X) / r.X;
                else if (r.Y != 0) minT = (q.Y - a.Y) / r.Y;
                else if (r.Z != 0) minT = (q.Z - a.Z) / r.Z;
            }

            //Me desplazo por el rayo hasta que su altura sea menor a la del terreno en ese punto
            //o me salga del AABB.
            float t = 0;
            float step = 1;

            for (t = minT; ; t += step)
            {
                collisionPoint = a + t * r;
                float y;

                if (!interpoledIntensity(collisionPoint.X, collisionPoint.Z, out y))
                    return false;

                if (collisionPoint.Y <= y + float.Epsilon)
                {
                    collisionPoint.Y = y;
                    collisionPoint = TGCVector3.TransformCoordinate(collisionPoint + traslation,
                        TGCMatrix.Scaling(ScaleXZ, ScaleY, ScaleXZ));
                    return true;
                }
            }
        }

        /// <summary>
        ///     Retorna true si hubo interseccion con el plano del terreno y setea el collisionPoint con la altura en ese punto.
        /// </summary>
        /// <param name="ray"></param>
        /// <param name="collisionPoint"></param>
        /// <returns></returns>
        public bool intersectRayTGCPlane(TgcRay ray, out TGCVector3 collisionPoint)
        {
            collisionPoint = TGCVector3.Empty;
            var minHeight = (minIntensity + traslation.Y) * ScaleY;

            float t;
            //Me fijo si intersecta con el BB del terreno.
            if (!TgcCollisionUtils.intersectRayPlane(ray, new TGCPlane(0, 1, 0, -minHeight), out t, out collisionPoint))
                return false;

            var collisionPointY = collisionPoint.Y;
            var interseccion = interpoledHeight(collisionPoint.X, collisionPoint.Z, out collisionPointY);
            collisionPoint.Y = collisionPointY;
            return interseccion;
        }

        /// <summary>
        ///     Transforma coordenadas del mundo en coordenadas del heightmap.
        /// </summary>
        public bool xzToHeightmapCoords(float x, float z, out TGCVector2 coords)
        {
            float i, j;

            i = x / ScaleXZ - traslation.X;
            j = z / ScaleXZ - traslation.Z;

            coords = new TGCVector2(i, j);

            if (coords.X >= HeightmapData.GetLength(0) || coords.Y >= HeightmapData.GetLength(1) || coords.Y < 0 ||
                coords.X < 0) return false;

            return true;
        }

        /// <summary>
        ///     Retorna la altura del terreno en ese punto utilizando interpolacion bilineal.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool interpoledHeight(float x, float z, out float y)
        {
            TGCVector2 coords;
            float i;
            y = 0;

            if (!xzToHeightmapCoords(x, z, out coords)) return false;
            interpoledIntensity(coords.X, coords.Y, out i);

            y = (i + traslation.Y) * ScaleY;
            return true;
        }

        /// <summary>
        ///     Retorna la intensidad del heightmap en ese punto utilizando interpolacion bilineal.
        /// </summary>
        /// <param name="u"></param>
        /// <param name="v"></param>
        /// <param name="i"></param>
        /// <returns></returns>
        public bool interpoledIntensity(float u, float v, out float i)
        {
            i = 0;

            float maxX = HeightmapData.GetLength(0);
            float maxZ = HeightmapData.GetLength(1);
            if (u >= maxX || v >= maxZ || v < 0 || u < 0) return false;

            int x1, x2, z1, z2;
            float s, t;

            x1 = (int)FastMath.Floor(u);
            x2 = x1 + 1;
            s = u - x1;

            z1 = (int)FastMath.Floor(v);
            z2 = z1 + 1;
            t = v - z1;

            if (z2 >= maxZ) z2--;
            if (x2 >= maxX) x2--;

            var i1 = HeightmapData[x1, z1] + s * (HeightmapData[x2, z1] - HeightmapData[x1, z1]);
            var i2 = HeightmapData[x1, z2] + s * (HeightmapData[x2, z2] - HeightmapData[x1, z2]);

            i = i1 + t * (i2 - i1);
            return true;
        }

        public List<TGCVector2> getPositionRangeXZGivenY(float ypos)
        {
            List<TGCVector2> coords = new List<TGCVector2>();

            float deltaY = 5;

            for (int row = 0; row < HeightmapData.GetLength(0); row++)
            {
                for (int column = 0; column < HeightmapData.GetLength(1); column++)
                {
                    interpoledIntensity(row, column, out float yPosHeightmap);
                    var possibleY = (yPosHeightmap + traslation.Y) * ScaleY;

                    if (ypos - deltaY < possibleY && ypos + deltaY > possibleY)
                    {                       
                        coords.Add(new TGCVector2(row, column));
                        row += 10;
                        column += 10;
                    }
                }
            }

            for(int index = 0; index < coords.Count(); index++)
                 coords[index] = xzWorldToHeightmap(coords[index].X, coords[index].Y);
            
            return coords;
        }    

        public TGCVector2 xzWorldToHeightmap(float x, float z)
        {
            var WorldPosX = (x + traslation.X) * ScaleXZ;
            var WorldPosZ = (z + traslation.Z) * ScaleXZ;

            var sizeX = HeightmapData.GetLength(0) * ScaleXZ / 2;
            var sizeZ = HeightmapData.GetLength(1) * ScaleXZ / 2;

            WorldPosX = FastMath.Clamp(WorldPosX, -sizeX + 200, sizeX - 200);
            WorldPosZ = FastMath.Clamp(WorldPosZ, -sizeZ + 200, sizeZ - 200);

            return new TGCVector2(WorldPosX, WorldPosZ);

        }

        public float convertToWorld(float pos)
        {
            return (pos + traslation.X) * ScaleXZ;
        }

        /// <summary>
        ///     Coloca el objeto a la altura correspondiente segun su posicion en el terreno.
        ///     Retorna false si esta fuera del terreno.
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        public bool setObjectPosition(ITransformObject o)
        {
            float y;
            if (!interpoledHeight(o.Position.X, o.Position.Z, out y)) return false;
            o.Position = new TGCVector3(o.Position.X, y, o.Position.Z);
            return true;
        }

        public TGCVector3 NormalVectorGivenXZ(float X, float Z)
        {
            float delta = 0.3f;

            interpoledHeight(X, Z + delta, out float alturaN);
            interpoledHeight(X, Z - delta, out float alturaS);
            interpoledHeight(X + delta, Z, out float alturaE);
            interpoledHeight(X - delta, Z, out float alturaO);

            TGCVector3 vectorEO = new TGCVector3(delta * 2, alturaE - alturaO, 0);
            TGCVector3 vectorNS = new TGCVector3(0, alturaN - alturaS, delta * 2);

            return TGCVector3.Cross(vectorNS, vectorEO);
        }

        public void AdaptToSurface(ITransformObject o)
        {
            var normalObjeto = NormalVectorGivenXZ(o.Position.X, o.Position.Z);

            var objectInclinationX = FastMath.Atan2(normalObjeto.X, normalObjeto.Y) * -FastMath.Sin(0);
            var objectInclinationZ = FastMath.Atan2(normalObjeto.X, normalObjeto.Y) * FastMath.Cos(0);

            float rotationX = -objectInclinationX;
            float rotationZ = -objectInclinationZ;

            o.RotateX(rotationX);
            o.RotateZ(rotationZ);
        }

        #endregion Utils
    }
}