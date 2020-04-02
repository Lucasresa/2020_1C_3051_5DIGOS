using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Utils;
using System.Collections.Generic;
using System.Windows.Forms;
using TGC.Tools.TerrainEditor;
using TGC.Group.Model.Corales;
using System;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        private const float SCALEXZ = 20f;
        private const float SCALEY = 8.4f;

        private SmartTerrain terrainHeightmap;
        private SmartTerrain waterHeightmap;
        private string pathTerrain;
        private string pathTextureTerrain;
        private string pathWater;
        private string pathTextureWater;
        
        private TgcScene navecita;
        private TgcScene roomNavecita;
        private List<Coral> corales;

        private CoralBuilder coralBuilder;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            coralBuilder = new CoralBuilder(mediaDir);
        }

        private float time;

        /// <summary>
        ///     Posicion inicial en el mapa
        /// </summary>
        public TGCVector3 PosicionInicialCamara { get; set; }

        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            // Creacion del terreno del juego
            terrainHeightmap = new SmartTerrain();

            // Path del heigthmap del terreno
            pathTerrain = MediaDir + "Heightmaps\\" + "suelo.jpg";

            // Textura del terreno
            pathTextureTerrain = MediaDir + "Textures\\" + "arena.jpg";

            terrainHeightmap.loadHeightmap(pathTerrain, SCALEXZ, SCALEY, TGCVector3.Empty);
            terrainHeightmap.loadTexture(pathTextureTerrain);

            //MessageBox.Show("Ancho X: " + terrainHeightmap.HeightmapData.GetLength(0).ToString() + "- Ancho Z: " +
            //                                                terrainHeightmap.HeightmapData.GetLength(1).ToString());
            waterHeightmap = new SmartTerrain();

            // Path del heigthmap del oceano
            pathWater = MediaDir + "Heightmaps\\" + "oceano.jpg";

            // Textura del oceano
            pathTextureWater = MediaDir + "Textures\\" + "agua.jpg";

            // Esto es para que tengan el mismo size ambos terrenos (despues hay que fixear esto haciendo terrenos del mismo size)
            var factor = 0.5859375f;
            waterHeightmap.loadHeightmap(pathWater, SCALEXZ * factor, 1, new TGCVector3(0, 3500, 0));
            waterHeightmap.loadTexture(pathTextureWater);

            // Inicializar camara
            Camara = new CamaraFPS(Input);

            // Instanciar navecita
            navecita = new TgcSceneLoader().loadSceneFromFile(MediaDir + "navecita-TgcScene.xml", MediaDir + "\\");
            navecita.Meshes.ForEach(parte => {
                parte.Scale = new TGCVector3(10.5f, 10.5f, 10.5f);
                parte.Position = new TGCVector3(350, 5500, 45);
            });


            // Instanciar Corales
            // TODO: Esto habria que derivarselo a otro objeto
            corales = coralBuilder.BuildCorals(terrainHeightmap, 100, new TGCVector4(-3000, 3000, -3000, 3000));            

            // TODO: La habitacion no hay que mostrarlar, ahora esta cargandola para probarla.
            // Prueba de instanciacion de la habitacion de la navecita
            roomNavecita = new TgcSceneLoader().loadSceneFromFile(MediaDir + "RoomNavecita-TgcScene.xml", MediaDir + "\\");
            roomNavecita.Meshes.ForEach(paredes => {
                paredes.Scale = new TGCVector3(10.5f, 10.5f, 10.5f);
                paredes.Position = new TGCVector3(350, 7500, -45);
            });
        }

        public override void Update()
        {
            PreUpdate();
            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            time += ElapsedTime;

            // Dibuja un texto por pantalla
            DrawText.drawText("Prueba de la camara q3fps", 0, 20, Color.OrangeRed);
            DrawText.drawText("camPos: [" + Camara.Position.ToString() + "]", 0, 40, Color.OrangeRed);
            DrawText.drawText("camLookAt: [" + Camara.LookAt.ToString() + "]", 0, 110, Color.OrangeRed);
            DrawText.drawText("Time: [" + time.ToString() + "]", 0, 180, Color.OrangeRed);            

            terrainHeightmap.render();
            waterHeightmap.render();            
            navecita.RenderAll();            
            roomNavecita.RenderAll();
            corales.ForEach( coral =>  coral.Render() );
            PostRender();
        }

        public override void Dispose()
        {
            navecita.DisposeAll();
            roomNavecita.DisposeAll();
            terrainHeightmap.dispose();
            waterHeightmap.dispose();
        }

        private float ObtenerMaximaAlturaTerreno()
        {
            var maximo = 0f;
            for (int x = 0; x < terrainHeightmap.HeightmapData.GetLength(0); x++)
            {
                for (int z = 0; z < terrainHeightmap.HeightmapData.GetLength(0); z++)
                {
                    var posibleMaximo = terrainHeightmap.HeightmapData[x, z];
                    if (maximo < terrainHeightmap.HeightmapData[x, z])
                    {
                        maximo = posibleMaximo;
                    }
                }
            }
            return maximo;
        }

    }
}
