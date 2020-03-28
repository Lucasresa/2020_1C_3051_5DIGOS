using Microsoft.DirectX.Direct3D;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Heightmap
{
    public class Heightmap : TgcExample
    {
        private string rutaTerreno;
        private float scaleXZ;
        private float scaleY;
        private string rutaTextura;
        private int totalVertices;
        private VertexBuffer vbTerreno;
        private Texture texturaTerreno;

        public Heightmap(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = "Mesh";
            Name = "Creacion de Heighmap";
            Description = "Creacion de un terreno en base a una Heighmap";
        }

        public override void Init()
        {
            // Detalles del terreno
            // Path del heigthmap del terreno
            rutaTerreno = MediaDir + "Heightmaps\\" + "suelo.jpg";

            // Determinar escala
            scaleXZ = 50f;
            scaleY = 1.5f;

            // Textura del terreno
            rutaTextura = MediaDir + "Textures\\" + "arena.jpg";

            // Crear Heigthmap
            CrearHeigthmap(D3DDevice.Instance.Device, rutaTerreno, scaleXZ, scaleY, rutaTextura);
        }

        private void CargarTexture(Device d3dDevice, string path)
        {
            //Rotar e invertir textura
            var b = (Bitmap)Image.FromFile(path);
            b.RotateFlip(RotateFlipType.Rotate90FlipX);
            texturaTerreno = Texture.FromBitmap(d3dDevice, b, Usage.None, Pool.Managed);
        }

        private void CrearHeigthmap(Device device, string terreno, float scaleXZ, float scaleY, string texturaTerreno)
        {
            // Parsear bitmap y cargar matriz de alturas
            var heigthmap = CargarHeigthMap(terreno);

            //Crear vertexBuffer
            totalVertices = 2 * 3 * (heigthmap.GetLength(0) - 1) * (heigthmap.GetLength(1) - 1);
            vbTerreno = new VertexBuffer(typeof(CustomVertex.PositionTextured), totalVertices, device, Usage.Dynamic | Usage.WriteOnly, CustomVertex.PositionTextured.Format, Pool.Default);

            //Crear array temporal de vertices
            var dataIdx = 0;
            var data = new CustomVertex.PositionTextured[totalVertices];


            //Iterar sobre toda la matriz del Heightmap y crear los triangulos necesarios para el terreno
            for (var i = 0; i < heigthmap.GetLength(0) - 1; i++)
            {
                for (var j = 0; j < heigthmap.GetLength(1) - 1; j++)
                {
                    //Crear los cuatro vertices que conforman este cuadrante, aplicando la escala correspondiente
                    var v1 = new TGCVector3(i * scaleXZ, heigthmap[i, j] * scaleY, j * scaleXZ);
                    var v2 = new TGCVector3(i * scaleXZ, heigthmap[i, j + 1] * scaleY, (j + 1) * scaleXZ);
                    var v3 = new TGCVector3((i + 1) * scaleXZ, heigthmap[i + 1, j] * scaleY, j * scaleXZ);
                    var v4 = new TGCVector3((i + 1) * scaleXZ, heigthmap[i + 1, j + 1] * scaleY, (j + 1) * scaleXZ);

                    //Crear las coordenadas de textura para los cuatro vertices del cuadrante
                    var t1 = new TGCVector2(i / (float)heigthmap.GetLength(0), j / (float)heigthmap.GetLength(1));
                    var t2 = new TGCVector2(i / (float)heigthmap.GetLength(0), (j + 1) / (float)heigthmap.GetLength(1));
                    var t3 = new TGCVector2((i + 1) / (float)heigthmap.GetLength(0), j / (float)heigthmap.GetLength(1));
                    var t4 = new TGCVector2((i + 1) / (float)heigthmap.GetLength(0), (j + 1) / (float)heigthmap.GetLength(1));

                    //Cargar triangulo 1
                    data[dataIdx] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 1] = new CustomVertex.PositionTextured(v2, t2.X, t2.Y);
                    data[dataIdx + 2] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);

                    //Cargar triangulo 2
                    data[dataIdx + 3] = new CustomVertex.PositionTextured(v1, t1.X, t1.Y);
                    data[dataIdx + 4] = new CustomVertex.PositionTextured(v4, t4.X, t4.Y);
                    data[dataIdx + 5] = new CustomVertex.PositionTextured(v3, t3.X, t3.Y);

                    dataIdx += 6;
                }
            }

            //Llenar todo el VertexBuffer con el array temporal
            vbTerreno.SetData(data, 0, LockFlags.None);

            CargarTexture(device, rutaTextura);

        }

        private int[,] CargarHeigthMap(string path)
        {
            //Cargar bitmap desde el FileSystem
            var bitmap = (Bitmap)Image.FromFile(path);
            var width = bitmap.Size.Width;
            var height = bitmap.Size.Height;
            var heigthmap = new int[width, height];

            for (var i = 0; i < width; i++)
            {
                for (var j = 0; j < height; j++)
                {
                    //Obtener color
                    //(j, i) invertido para primero barrer filas y despues columnas
                    var pixel = bitmap.GetPixel(j, i);

                    //Calcular intensidad en escala de grises
                    var intensity = pixel.R * 0.299f + pixel.G * 0.587f + pixel.B * 0.114f;
                    heigthmap[i, j] = (int)intensity;
                }
            }

            return heigthmap;
        }

        public override void Update()
        {
            PreUpdate();
            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            // Renderizar el terreno
            D3DDevice.Instance.Device.SetTexture(0, texturaTerreno);
            D3DDevice.Instance.Device.VertexFormat = CustomVertex.PositionTextured.Format;
            D3DDevice.Instance.Device.SetStreamSource(0, vbTerreno, 0);
            D3DDevice.Instance.Device.DrawPrimitives(PrimitiveType.TriangleList, 0, totalVertices / 3);

            PostRender();
        }

        public override void Dispose()
        {
            texturaTerreno.Dispose();
            vbTerreno.Dispose();
        }
    }
}
