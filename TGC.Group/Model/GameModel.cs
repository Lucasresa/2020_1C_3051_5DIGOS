using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        private const float SCALEXZ = 20f;
        private const float SCALEY = 10.4f;

        private TgcSimpleTerrain terrainHeightmap;
        private TgcSimpleTerrain waterHeightmap;
        private string pathTerrain;
        private string pathTextureTerrain;
        private string pathWater;
        private string pathTextureWater;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            // Creacion del terreno del juego
            terrainHeightmap = new TgcSimpleTerrain();

            // Path del heigthmap del terreno
            pathTerrain = MediaDir + "Heightmaps\\" + "suelo.jpg";

            // Textura del terreno
            pathTextureTerrain = MediaDir + "Textures\\" + "arena.jpg";

            terrainHeightmap.loadHeightmap(pathTerrain, SCALEXZ, SCALEY, new TGCVector3(0, 0, 0));
            terrainHeightmap.loadTexture(pathTextureTerrain);

            waterHeightmap = new TgcSimpleTerrain();

            // Path del heigthmap del oceano
            pathWater = MediaDir + "Heightmaps\\" + "oceano.jpg";

            // Textura del oceano
            pathTextureWater = MediaDir + "Textures\\" + "agua.jpg";

            //esto es para que tengan el mismo size ambos terrenos (despues hay que fixear esto haciendo terrenos del mismo size)
            var factor = 0.5859375f;
            waterHeightmap.loadHeightmap(pathWater, SCALEXZ*factor, 1, new TGCVector3(0, 2500, 0));
            waterHeightmap.loadTexture(pathTextureWater);   

            // Inicializar camara          
            Camara = new CamaraFPS(Input);

        }

        public override void Update()
        {
            PreUpdate();
            //Camara.UpdateCamera(ElapsedTime);
            PostUpdate();
        }


        public override void Render()
        {
            PreRender();

            // Dibuja un texto por pantalla
            DrawText.drawText("Prueba del terreno junto al oceano", 0, 20, Color.OrangeRed);

            // Render de los terrenos
            terrainHeightmap.Render();
            waterHeightmap.Render();
            
            PostRender();
        }

        public override void Dispose()
        {
            terrainHeightmap.Dispose();
            waterHeightmap.Dispose();
        }
    }
}
