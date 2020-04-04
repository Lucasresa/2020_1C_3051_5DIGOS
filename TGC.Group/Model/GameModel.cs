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
using TGC.Group.Model.Corales;
using System;
using TGC.Group.Model.Minerals;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Sharky;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        private float time;
        private TgcScene navecita;
        // private TgcScene roomNavecita;
        private List<Coral> corales;
        private List<Ore> minerals;

        private Sky skyBox;

        private CoralBuilder coralBuilder;
        private OreBuilder oreBuilder;
        private World terrain;
        private World water;
        private Shark shark;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            coralBuilder = new CoralBuilder(mediaDir);
            oreBuilder = new OreBuilder(mediaDir);
        }

        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;

            Camara = new CamaraFPS(Input);

            terrain = new Terrain(MediaDir, ShadersDir);
            water = new Water(MediaDir, ShadersDir);
            
            terrain.LoadWorld(TGCVector3.Empty);
            water.LoadWorld(new TGCVector3(0, 3500, 0));

            // TODO: Habria que encontrar imagenes con mayor resolucion para el SkyBox
            skyBox = new Sky(MediaDir, ShadersDir);
            skyBox.Init();

            shark = new Shark(MediaDir, ShadersDir);
            shark.loadMesh();
            
            // TODO: Hay que corregir la posicion de la nave y lo ideal seria utilizando una transformacion
            navecita = new TgcSceneLoader().loadSceneFromFile(MediaDir + "navecita-TgcScene.xml", MediaDir + "\\");
            navecita.Meshes.ForEach(parte => {
                parte.Scale = new TGCVector3(10, 10, 10);
                parte.Position = new TGCVector3(530, 3630, 100);
                parte.Rotation = new TGCVector3(-13, 1, 270);
            });

            Tuple<float, float> positionRangeXCoral = new Tuple<float, float>(-3000, 3000);
            Tuple<float, float> positionRangeZCoral = new Tuple<float, float>(-3000, 3000);

            corales = coralBuilder.CreateRandomCorals(50, positionRangeXCoral, positionRangeZCoral);
            coralBuilder.LocateCoralsInTerrain(terrain.world, corales);

            Tuple<float, float> positionRangeXOre = new Tuple<float, float>(-3000, 3000);
            Tuple<float, float> positionRangeZOre = new Tuple<float, float>(-3000, 3000);

            minerals = oreBuilder.CreateRandomMinerals(50, positionRangeXOre, positionRangeZOre);
            oreBuilder.LocateMineralsInTerrain(terrain.world, minerals);

            // TODO: La habitacion no hay que mostrarlar, ahora esta cargandola para probarla.
            // Prueba de instanciacion de la habitacion de la navecita
            //roomNavecita = new TgcSceneLoader().loadSceneFromFile(MediaDir + "RoomNavecita-TgcScene.xml", MediaDir + "\\");
            //roomNavecita.Meshes.ForEach(paredes => {
            //    paredes.Scale = new TGCVector3(10.5f, 10.5f, 10.5f);
            //    paredes.Position = new TGCVector3(350, 7500, -45);
            //});
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
            DrawText.drawText("Prueba de ubicacion de objetos en el terreno", 0, 20, Color.Red);
            DrawText.drawText("camPos: [" + Camara.Position.X.ToString() + "; "
                                          + Camara.Position.Y.ToString() + "; "
                                          + Camara.Position.Z.ToString() + "] ",
                              0, 60, Color.DarkRed);
            DrawText.drawText("camLookAt: [" + Camara.LookAt.X.ToString() + "; "
                                          + Camara.LookAt.Y.ToString() + "; "
                                          + Camara.LookAt.Z.ToString() + "] ",
                              0, 80, Color.DarkRed);

            DrawText.drawText("TIME: [" + time.ToString() + "]", 0, 100, Color.DarkRed);

            terrain.Render();
            water.Render();

            shark.Render();

            navecita.RenderAll();            
            //roomNavecita.RenderAll();

            skyBox.Render();
            
            corales.ForEach(coral =>
            {
                coral.Mesh.UpdateMeshTransform();
                coral.Render();
            });

            minerals.ForEach(ore =>
            {
                ore.Mesh.UpdateMeshTransform();
                ore.Render();

            });

            //coral0.Render();
            //coral1.Render();
            //coral2.Render();
            PostRender();
        }

        public override void Dispose()
        {
            navecita.DisposeAll();
            //roomNavecita.DisposeAll();
            
            terrain.Dispose();
            water.Dispose();

            shark.Dispose();

            corales.ForEach(coral => coral.Dispose());
            minerals.ForEach(ore => ore.Dispose());
        }

       /* private float ObtenerMaximaAlturaTerreno()
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
        */
    }
}
