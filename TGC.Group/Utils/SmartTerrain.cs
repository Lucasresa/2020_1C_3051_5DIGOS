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
        #region Atributos
        public float length { get; protected set; }
        public float width { get; protected set; }
        private float maxIntensity = 0;
        private float minIntensity = -1;
        private TGCVector3 traslation;
        private VertexBuffer vbTerrain;
        private CustomVertex.PositionTextured[] vertices;
        public CustomVertex.PositionTextured[] getVertices() { return vertices; }
        private Texture terrainTexture;
        public int TotalVertices { get; private set; }
        public float[,] HeightmapData { get; private set; }
        public bool Enabled { get; set; }
        private TGCVector3 center;
        public TGCVector3 Center { get => center; set => center = value; }
        public bool AlphaBlendEnable { get; set; }
        protected Effect effect;
        public Effect Effect { get => effect; set => effect = value; }
        protected string technique;
        public string Technique { get => technique; set => technique = value; }
        public float ScaleXZ { get; set; }
        public float ScaleY { get; set; }
        #endregion

        #region Constructor
        public SmartTerrain()
        {
            Enabled = true;
            AlphaBlendEnable = false;
            effect = TGCShaders.Instance.VariosShader;
            technique = TGCShaders.T_POSITION_TEXTURED;
        }
        #endregion       

        #region Metodos

        public void loadTexture(string path)
        {
            if (terrainTexture != null && !terrainTexture.Disposed)
                terrainTexture.Dispose();

            var bitMap = (Bitmap)Image.FromFile(path);
            bitMap.RotateFlip(RotateFlipType.Rotate90FlipX);
            terrainTexture = Texture.FromBitmap(D3DDevice.Instance.Device, bitMap, Usage.AutoGenerateMipMap, Pool.Managed);
            bitMap.Dispose();
        }

        protected float[,] loadHeightMap(string path)
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

        public void setHeightmapData(float[,] heightmapData)
        {
            if (heightmapData.GetLength(0) == HeightmapData.GetLength(0) && HeightmapData.GetLength(1) == heightmapData.GetLength(1))
                HeightmapData = heightmapData;
        }

        public void loadHeightmap(string heightmapPath, float scaleXZ, float scaleY, TGCVector3 center)
        {
            Center = center;
            ScaleXZ = scaleXZ;
            ScaleY = scaleY;

            if (vbTerrain != null && !vbTerrain.Disposed)
                vbTerrain.Dispose();

            HeightmapData = loadHeightMap(heightmapPath);
            width = HeightmapData.GetLength(0);
            length = HeightmapData.GetLength(1);
            var totalvertices = 2 * 3 * (width - 1) * (length - 1);
            TotalVertices = (int)totalvertices;

            vbTerrain = new VertexBuffer(typeof(CustomVertex.PositionTextured), TotalVertices,
                                         D3DDevice.Instance.Device,
                                         Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            loadVertices();
        }

        private void loadVertices()
        {
            traslation.X = center.X - width / 2;
            traslation.Y = center.Y;
            traslation.Z = center.Z - length / 2;

            var dataIdx = 0;

            vertices = new CustomVertex.PositionTextured[TotalVertices];

            for (var i = 0; i < width - 1; i++)
            {
                for (var j = 0; j < length - 1; j++)
                {
                    if (HeightmapData[i, j] > maxIntensity) maxIntensity = HeightmapData[i, j];
                    if (minIntensity == -1 || HeightmapData[i, j] < minIntensity) minIntensity = HeightmapData[i, j];

                    //Vertices
                    var v1 = new TGCVector3((traslation.X + i) * ScaleXZ, (traslation.Y + HeightmapData[i, j]) * ScaleY, (traslation.Z + j) * ScaleXZ);
                    var v2 = new TGCVector3((traslation.X + i) * ScaleXZ, (traslation.Y + HeightmapData[i, j + 1]) * ScaleY, (traslation.Z + (j + 1)) * ScaleXZ);
                    var v3 = new TGCVector3((traslation.X + i + 1) * ScaleXZ, (traslation.Y + HeightmapData[i + 1, j]) * ScaleY, (traslation.Z + j) * ScaleXZ);
                    var v4 = new TGCVector3((traslation.X + i + 1) * ScaleXZ, (traslation.Y + HeightmapData[i + 1, j + 1]) * ScaleY, (traslation.Z + j + 1) * ScaleXZ);

                    //Coordendas de textura
                    var t1 = new TGCVector2(i / width, j / length);
                    var t2 = new TGCVector2(i / width, (j + 1) / length);
                    var t3 = new TGCVector2((i + 1) / width, j / length);
                    var t4 = new TGCVector2((i + 1) / width, (j + 1) / length);

                    //Cargar triangulo 1
                    vertices[dataIdx + 0] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    vertices[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
                    vertices[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

                    //Cargar triangulo 2
                    vertices[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    vertices[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
                    vertices[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

                    dataIdx += 6;
                }
                vbTerrain.SetData(vertices, 0, LockFlags.None);
            }
        }

        public void updateVertices()
        {
            for (var i = 0; i < vertices.Length; i++)
            {
                var intensity = HeightmapData[(int)vertices[i].X, (int)vertices[i].Z];
                vertices[i].Y = intensity;
                if (intensity > maxIntensity) maxIntensity = intensity;
                if (minIntensity == -1 || intensity < minIntensity) minIntensity = intensity;
            }

            vbTerrain.SetData(vertices, 0, LockFlags.None);
        }

        public void Render()
        {
            if (!Enabled) return;

            var d3dDevice = D3DDevice.Instance.Device;
            var texturesManager = TexturesManager.Instance;
            var shader = TGCShaders.Instance;
            // TODO 0 : var transform = TGCMatrix.Translation(traslation) * TGCMatrix.Scaling(ScaleXZ, ScaleY, ScaleXZ);

            effect.SetValue("texDiffuseMap", terrainTexture);
            texturesManager.clear(1);
            // TODO 1 : shader.SetShaderMatrix(effect, transform);
            shader.SetShaderMatrix(effect, TGCMatrix.Identity);
            d3dDevice.VertexDeclaration = shader.VdecPositionTextured;
            effect.Technique = technique;
            d3dDevice.SetStreamSource(0, vbTerrain, 0);

            // TODO 2 : effect.Begin(0)
            /* var p = effect.Begin(0);
            for (var i = 0; i < p; i++)
            {
                effect.BeginPass(i);
                d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, TotalVertices / 3);
                effect.EndPass();
            }
            effect.End();           
            */

            effect.Begin(0);
            effect.BeginPass(0);
            d3dDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, TotalVertices / 3);
            effect.EndPass();
            effect.End();

        }

        public void Dispose()
        {
            if (vbTerrain != null) vbTerrain.Dispose();
            if (terrainTexture != null) terrainTexture.Dispose();
        }

        #endregion 

        #region Utils

        public bool xzToHeightmapCoords(float x, float z, out TGCVector2 coords)
        {
            float i = x / ScaleXZ - traslation.X;
            float j = z / ScaleXZ - traslation.Z;

            coords = new TGCVector2(i, j);

            if (coords.X >= HeightmapData.GetLength(0) || coords.Y >= HeightmapData.GetLength(1) || coords.Y < 0 || coords.X < 0)
                return false;
            return true;
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

        public bool interpoledHeight(float x, float z, out float y)
        {
            TGCVector2 coords;
            y = 0;

            if (!xzToHeightmapCoords(x, z, out coords)) return false;

            interpoledIntensity(coords.X, coords.Y, out float i);

            y = (i + traslation.Y) * ScaleY;
            return true;
        }

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

        public bool setObjectPosition(ITransformObject o)
        {
            if (!interpoledHeight(o.Position.X, o.Position.Z, out float y)) return false;
            o.Position = new TGCVector3(o.Position.X, y, o.Position.Z);
            return true;
        }

        #endregion Utils

    }
}