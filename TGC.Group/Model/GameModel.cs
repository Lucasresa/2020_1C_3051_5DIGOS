using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;
using System.Collections.Generic;
using System;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.MeshBuilders;
using TGC.Group.Model.Watercraft;
using static TGC.Group.Model.Terrains.Terrain;
using System.Runtime.CompilerServices;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        #region Constantes
        private struct Constants
        {
            public static float WATER_HEIGHT = 3500;
            public static TGCVector3 OUTSIDE_SHIP_POSITION = new TGCVector3(1300, 3505, 20);
            public static TGCVector3 INSIDE_SHIP_POSITION = new TGCVector3(515, -2338, -40);
        }
        #endregion

        #region Atributos
        private float time;
        private List<TgcMesh> corales = new List<TgcMesh>();
        private List<TgcMesh> minerals = new List<TgcMesh>();
        private List<TgcMesh> vegetation = new List<TgcMesh>();
        private List<TgcMesh> fishes = new List<TgcMesh>();
        private Sky skyBox;
        private Perimeter currentCameraArea;
        private Ship ship;
        private Terrain terrain;
        private Water water;
        private Shark shark;
        private MeshBuilder meshBuilder;
        private bool showDebugInfo { get; set; }
        #endregion

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            meshBuilder = new MeshBuilder();
            MeshDuplicator.MediaDir = mediaDir;
            D3DDevice.Instance.ZFarPlaneDistance = 8000f;
        }

        public override void Init()
        {            
            #region Camera 
            Camera = new CameraFPS(Input, Constants.INSIDE_SHIP_POSITION);   
            #endregion

            #region Mundo            
            terrain = new Terrain(MediaDir, ShadersDir);
            terrain.LoadWorld(TGCVector3.Empty);
            terrain.splitToArea();
            water = new Water(MediaDir, ShadersDir);
            water.LoadWorld(new TGCVector3(0, Constants.WATER_HEIGHT, 0));
            skyBox = new Sky(MediaDir, ShadersDir, Camera);
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
            MeshDuplicator.InitOriginalMeshes();
            meshInitializer();
            #endregion

        }

        public override void Update()
        {
            PreUpdate();

            currentCameraArea = terrain.getArea(Camera.Position.X, Camera.Position.Z);

            #region Teclas

            if (Input.keyPressed(Key.F))
                showDebugInfo = !showDebugInfo;

            if (Input.keyPressed(Key.E) && CameraInRoom())
            {
                ((CameraFPS)Camera).movementSpeed = 4;
                ((CameraFPS)Camera).TeleportCamera(Constants.OUTSIDE_SHIP_POSITION);
            }

            #endregion

            PostUpdate();
        }

        public override void Render()
        {
            PreRender();

            #region Texto en pantalla

            time += ElapsedTime;

            if (showDebugInfo)
            {
                DrawText.drawText("DATOS DE LA Camera: ", 0, 30, Color.Red);
                DrawText.drawText("Posicion: [" + Camera.Position.X.ToString() + "; "
                                              + Camera.Position.Y.ToString() + "; "
                                              + Camera.Position.Z.ToString() + "] ",
                                  0, 60, Color.DarkRed);
                DrawText.drawText("Objetivo: [" + Camera.LookAt.X.ToString() + "; "
                                              + Camera.LookAt.Y.ToString() + "; "
                                              + Camera.LookAt.Z.ToString() + "] ",
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
            
            ship.Render();

            if (Camera.Position.Y > 0)
            {
                terrain.Render();
                water.Render();
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

                vegetation.ForEach(vegetation =>
                {
                    vegetation.AlphaBlendEnable = true;
                    vegetation.UpdateMeshTransform();
                    vegetation.Render();

                });

                shark.Render();
                fishes.ForEach(fish =>
                {
                    fish.UpdateMeshTransform();
                    fish.Render();

                });
            }

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
            MeshDuplicator.DisposeOriginalMeshes();
            #endregion
        }

        #region Metodos Privados
        private void meshInitializer()
        {
            var treeCorals = meshBuilder.CreateNewScaledMeshes(MeshType.treeCoral, 15, 10);
            meshBuilder.LocateMeshesInTerrain(ref treeCorals, terrain.SizeWorld(), terrain.world);
            var spiralCorals = meshBuilder.CreateNewScaledMeshes(MeshType.spiralCoral, 15, 10);
            meshBuilder.LocateMeshesInTerrain(ref spiralCorals, terrain.SizeWorld(), terrain.world);
            var goldOre = meshBuilder.CreateNewScaledMeshes(MeshType.goldOre, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref goldOre, terrain.SizeWorld(), terrain.world);
            var goldOreCommon = meshBuilder.CreateNewScaledMeshes(MeshType.goldOreCommon, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref goldOreCommon, terrain.SizeWorld(), terrain.world);
            var silverOre = meshBuilder.CreateNewScaledMeshes(MeshType.silverOre, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref silverOre, terrain.SizeWorld(), terrain.world);
            var silverOreCommon = meshBuilder.CreateNewScaledMeshes(MeshType.silverOreCommon, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref silverOreCommon, terrain.SizeWorld(), terrain.world);
            var ironOre = meshBuilder.CreateNewScaledMeshes(MeshType.ironOre, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref ironOre, terrain.SizeWorld(), terrain.world);
            var ironOreCommon = meshBuilder.CreateNewScaledMeshes(MeshType.ironOreCommon, 15, 5);
            meshBuilder.LocateMeshesInTerrain(ref ironOreCommon, terrain.SizeWorld(), terrain.world);
            var rock = meshBuilder.CreateNewScaledMeshes(MeshType.rock, 15, 8);
            meshBuilder.LocateMeshesInTerrain(ref rock, terrain.SizeWorld(), terrain.world);
            var alga = meshBuilder.CreateNewScaledMeshes(MeshType.alga, 365, 5);
            meshBuilder.LocateMeshesInTerrain(ref alga, terrain.SizeWorld(), terrain.world);
            var normalFish = meshBuilder.CreateNewScaledMeshes(MeshType.normalFish, 15, 5);
            meshBuilder.LocateMeshesUpToTerrain(ref normalFish, terrain.SizeWorld(), terrain.world, water.world.Center.Y - 200);
            var yellowFish = meshBuilder.CreateNewScaledMeshes(MeshType.yellowFish, 15, 5);
            meshBuilder.LocateMeshesUpToTerrain(ref yellowFish, terrain.SizeWorld(), terrain.world, water.world.Center.Y - 200);

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
            fishes.AddRange(normalFish);
            fishes.AddRange(yellowFish);
        }

        private bool CameraInRoom()
        {
            // TODO: Cambiar el delta cuando podamos construir el -BoundingBox- o el cuerpo rigido
            float delta = 300;
            return ship.InsideMesh.Position.Y - delta < Camera.Position.Y &&
                   Camera.Position.Y < ship.InsideMesh.Position.Y + delta;
        }
        #endregion
    }
}
