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
using TGC.Group.Model.Watercraft;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        #region Atributos
        private float time;
        private List<TgcMesh> corales = new List<TgcMesh>();
        private List<TgcMesh> minerals = new List<TgcMesh>();
        private List<TgcMesh> vegetation = new List<TgcMesh>();
        private List<Fish> fishes;
        private Sky skyBox;
        private Perimeter currentCameraArea;
        private Ship ship;
        private Tuple<float, float> positionRangeX = new Tuple<float, float>(-2900, 2900);
        private Tuple<float, float> positionRangeZ = new Tuple<float, float>(-2900, 2900);
        private FishBuilder fishBuilder;
        private World terrain;
        private World water;
        private Shark shark;
        private MeshBuilder meshBuilder;
        private bool showDebugInfo { get; set; }
        #endregion

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            fishBuilder = new FishBuilder(mediaDir);
            meshBuilder = new MeshBuilder();
            MeshDuplicator.MediaDir = mediaDir;
            D3DDevice.Instance.ZFarPlaneDistance = 16000f;
        }

        public override void Init()
        {            
            #region Camara 
            //TODO: Mover estas posiciones fijas a constantes y posteriormente a un archivo de config
            Camara = new CameraFPS(Input, new TGCVector3(515, -2338, -40));   
            #endregion

            #region Mundo            
            terrain = new Terrain(MediaDir, ShadersDir);
            terrain.LoadWorld(TGCVector3.Empty);
            terrain.splitToArea();
            water = new Water(MediaDir, ShadersDir);
            water.LoadWorld(new TGCVector3(0, 3500, 0));
            skyBox = new Sky(MediaDir, ShadersDir);
            skyBox.LoadSkyBox();
            #endregion
                        
            #region Nave
            ship = new Ship(MediaDir, ShadersDir);
            ship.LoadShip();
            #endregion

            #region Enemigo
            shark = new Shark(MediaDir, ShadersDir);
            shark.LoadShark();
            #endregion

            #region Vegetacion del mundo

            fishes = fishBuilder.CreateRandomFishes(30, positionRangeX, positionRangeZ);
            fishBuilder.LocateFishesInTerrain(terrain.world, fishes, water.world.Center.Y - 300);

            MeshDuplicator.InitOriginalMeshes();
            meshInitializer();

            #endregion

        }

        public override void Update()
        {
            PreUpdate();

            currentCameraArea = terrain.getArea(Camara.Position.X, Camara.Position.Z);

            if (Input.keyPressed(Key.F))
                showDebugInfo = !showDebugInfo;

            if (Input.keyPressed(Key.E) && camaraInRoom())
            {
                var position = new TGCVector3(1300, 3505, 20);
                Camara = new CameraFPS(Input, position);
            }

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            #region Texto en pantalla

            time += ElapsedTime;

            if (showDebugInfo)
            {
                DrawText.drawText("DATOS DE LA CAMARA: ", 0, 30, Color.Red);
                DrawText.drawText("Posicion: [" + Camara.Position.X.ToString() + "; "
                                              + Camara.Position.Y.ToString() + "; "
                                              + Camara.Position.Z.ToString() + "] ",
                                  0, 60, Color.DarkRed);
                DrawText.drawText("Objetivo: [" + Camara.LookAt.X.ToString() + "; "
                                              + Camara.LookAt.Y.ToString() + "; "
                                              + Camara.LookAt.Z.ToString() + "] ",
                                  0, 80, Color.DarkRed);
                DrawText.drawText("TIME: [" + time.ToString() + "]", 0, 100, Color.DarkRed);


                DrawText.drawText("DATOS DEL AREA ACTUAL: ", 0, 130, Color.Red);

                DrawText.drawText("RANGO DE X: " +
                                    "\nMinimo " + currentCameraArea.xMin.ToString() +
                                    "\nMaximo " + currentCameraArea.xMax.ToString() + "\n\n" +

                                  "RANGO DE Z: " +
                                    "\nMinimo " + currentCameraArea.zMin.ToString() +
                                    "\nMaximo " + currentCameraArea.zMax.ToString(),
                             0, 160, Color.DarkRed);
            }

            #endregion

            #region Renderizado

            terrain.Render();
            water.Render();
            skyBox.Render();
            ship.Render();

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

            vegetation.ForEach(vegetation =>
            {
                vegetation.AlphaBlendEnable = true;
                vegetation.UpdateMeshTransform();
                vegetation.Render();

            });

            shark.Render();
            fishes.ForEach(fish =>
            {
                fish.Mesh.UpdateMeshTransform();
                fish.Render();

            });

            #endregion

            PostRender();
        }

        public override void Dispose()
        {
            #region Liberacion de recursos

            ship.Dispose();
            terrain.Dispose();
            water.Dispose();
            skyBox.Dispose();
            shark.Dispose();
            fishes.ForEach(fish => fish.Dispose());
            corales.ForEach(coral => coral.Dispose());
            minerals.ForEach(ore => ore.Dispose());
            vegetation.ForEach(vegetation => vegetation.Dispose());

            #endregion
        }

        private void meshInitializer()
        {
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
            var alga = meshBuilder.CreateNewScaledMeshes(MeshType.alga, 200, 5);
            meshBuilder.LocateMeshesInTerrain(ref alga, positionRangeX, positionRangeZ, terrain.world);

            corales.AddRange(treeCorals);
            corales.AddRange(spiralCorals);
            minerals.AddRange(goldOre);
            minerals.AddRange(goldOreCommon);
            minerals.AddRange(silverOre);
            minerals.AddRange(silverOreCommon);
            minerals.AddRange(ironOre);
            minerals.AddRange(ironOreCommon);
            minerals.AddRange(rock);
            vegetation.AddRange(alga);
        }

        private bool camaraInRoom()
        {
            // TODO: Cambiar el delta cuando podamos construir el -BoundingBox-
            float delta = 300;
            return ship.InsideMesh.Position.Y - delta < Camara.Position.Y &&
                   Camara.Position.Y < ship.InsideMesh.Position.Y + delta;
        }
    }
}