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
using TGC.Group.Model.Fishes;
using TGC.Group.Model.MeshBuilders;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        private float time;
        private TgcScene navecita;
        // private TgcScene roomNavecita;
        private List<TgcMesh> corales = new List<TgcMesh>();
        private List<TgcMesh> minerals = new List<TgcMesh>();
        private List<Fish> fishes;
        private Sky skyBox;

        private FishBuilder fishBuilder;
        private World terrain;
        private World water;
        private Shark shark;

        private MeshBuilder meshBuilder;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            fishBuilder = new FishBuilder(mediaDir);
            meshBuilder = new MeshBuilder();
        }

        public override void Init()
        {
            MeshDuplicator.MediaDir = MediaDir;

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

            Tuple<float, float> positionRangeX = new Tuple<float, float>(-2900, 2900);
            Tuple<float, float> positionRangeZ = new Tuple<float, float>(-2900, 2900);

            fishes = fishBuilder.CreateRandomFishes(30, positionRangeX, positionRangeZ);
            fishBuilder.LocateFishesInTerrain(terrain.world, fishes, water.world.Center.Y);

            MeshDuplicator.InitOriginalMeshes();

            var normalCorals = meshBuilder.CreateNewScaledMeshes(MeshType.normalCoral, 33, 4);
            meshBuilder.LocateMeshesInTerrain(ref normalCorals, positionRangeX, positionRangeZ, terrain.world);
            var treeCorals = meshBuilder.CreateNewScaledMeshes(MeshType.treeCoral, 33, 10);
            meshBuilder.LocateMeshesInTerrain(ref treeCorals, positionRangeX, positionRangeZ, terrain.world);
            var spiralCorals = meshBuilder.CreateNewScaledMeshes(MeshType.spiralCoral, 33, 10);
            meshBuilder.LocateMeshesInTerrain(ref spiralCorals, positionRangeX, positionRangeZ, terrain.world);

            var goldOre = meshBuilder.CreateNewScaledMeshes(MeshType.goldOre, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref goldOre, positionRangeX, positionRangeZ, terrain.world);
            var goldOreCommon = meshBuilder.CreateNewScaledMeshes(MeshType.goldOreCommon, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref goldOreCommon, positionRangeX, positionRangeZ, terrain.world);
            var silverOre = meshBuilder.CreateNewScaledMeshes(MeshType.silverOre, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref silverOre, positionRangeX, positionRangeZ, terrain.world);
            var silverOreCommon = meshBuilder.CreateNewScaledMeshes(MeshType.silverOreCommon, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref silverOreCommon, positionRangeX, positionRangeZ, terrain.world);
            var ironOre = meshBuilder.CreateNewScaledMeshes(MeshType.ironOre, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref ironOre, positionRangeX, positionRangeZ, terrain.world);
            var ironOreCommon = meshBuilder.CreateNewScaledMeshes(MeshType.ironOreCommon, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref ironOreCommon, positionRangeX, positionRangeZ, terrain.world);
            var rock = meshBuilder.CreateNewScaledMeshes(MeshType.rock, 30, 8);
            meshBuilder.LocateMeshesInTerrain(ref rock, positionRangeX, positionRangeZ, terrain.world);

            corales.AddRange(normalCorals);
            corales.AddRange(treeCorals);
            corales.AddRange(spiralCorals);

            minerals.AddRange(goldOre);
            minerals.AddRange(goldOreCommon);
            minerals.AddRange(silverOre);
            minerals.AddRange(silverOreCommon);
            minerals.AddRange(ironOre);
            minerals.AddRange(ironOreCommon);
            minerals.AddRange(rock);

            //  minerals = meshBuilder.CreateNewScaledMeshes(MeshType.ironOre, 20, 5);
            //  meshBuilder.LocateMeshesInTerrain(ref minerals, positionRangeX, positionRangeZ, terrain.world);


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
                coral.UpdateMeshTransform();
                coral.Render();
            });

            minerals.ForEach(ore =>
            {
                ore.UpdateMeshTransform();
                ore.Render();

            });

            fishes.ForEach(fish =>
            {
                fish.Mesh.UpdateMeshTransform();
                fish.Render();

            });
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
            fishes.ForEach(fish => fish.Dispose());
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
