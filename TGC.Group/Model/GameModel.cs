using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Core.Camara;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        //private Heightmap heightmap;
        private TgcSimpleTerrain heightmap;
        private string rutaTerreno;
        private float scaleXZ;
        private float scaleY;
        private string rutaTextura;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;
            
            heightmap = new TgcSimpleTerrain();
            //heightmap = new Heightmap(MediaDir,ShadersDir);
            
            // Path del heigthmap del terreno
            rutaTerreno = MediaDir + "Heightmaps\\" + "suelo.jpg";

            // Determinar escala
            scaleXZ = 15f;
            scaleY = 10.40f;

            // Textura del terreno
            rutaTextura = MediaDir + "Textures\\" + "arena.jpg";

            //heightmap.CrearHeigthmap(d3dDevice, rutaTerreno, scaleXZ, scaleY, rutaTextura);

            heightmap.loadHeightmap(rutaTerreno, scaleXZ, scaleY, new TGCVector3(0, 0, 0));
            heightmap.loadTexture(rutaTextura);

            // Inicializar camara

            var cameraPosition = new TGCVector3(4500, 3600, 1100);
            var lookAt = new TGCVector3(1905, 1457, 45);
                 
            Camara.SetCamera(cameraPosition, lookAt);          
          
        }

        public override void Update()
        {
            PreUpdate();
            PostUpdate();
        }


        public override void Render()
        {
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Primera prueba", 0, 20, Color.OrangeRed);

            heightmap.Render();
                              
            PostRender();
        }

        public override void Dispose()
        {
            heightmap.Dispose();
        }
    }
}