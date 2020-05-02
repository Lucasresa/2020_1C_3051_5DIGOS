using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.MeshBuilders;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Watercraft;
using TGC.Group.Utils;
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        #region Constantes
        private struct Constants
        {
            public static float WATER_HEIGHT = 3500;
            public static TGCVector3 OUTSIDE_SHIP_POSITION = new TGCVector3(1300, 3505, 20);
            public static TGCVector3 INSIDE_SHIP_POSITION = new TGCVector3(515, -2340, -40);
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
        private CameraFPS camera;
        private readonly PhysicalWorld physicalworld = PhysicalWorld.Instance;
        private Bullet.RigidBody rigidBodies = new Bullet.RigidBody();
        #endregion

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FixedTickEnable = true;
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
            camera = (CameraFPS)Camera;
            camera.setShipPosition(Constants.INSIDE_SHIP_POSITION, Constants.OUTSIDE_SHIP_POSITION);
            #endregion

            #region Mundo            
            terrain = new Terrain(MediaDir, ShadersDir);
            terrain.LoadWorld(TGCVector3.Empty);
            terrain.splitToArea();
            water = new Water(MediaDir, ShadersDir);
            water.LoadWorld(new TGCVector3(0, Constants.WATER_HEIGHT, 0));
            skyBox = new Sky(MediaDir, ShadersDir, camera);
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

            #region Mundo fisico

            rigidBodies.Initializer(terrain, camera, shark, ship);
            physicalworld.addInitialRigidBodies(rigidBodies.rigidBodies);
            physicalworld.addAllDynamicsWorld();

            #endregion

            #region Vegetacion del mundo
            MeshDuplicator.InitOriginalMeshes();
            meshInitializer();
            #endregion
        }

        public override void Update()
        {
            physicalworld.Update(Input, ElapsedTime, TimeBetweenUpdates);

            currentCameraArea = terrain.getArea(camera.position.X, camera.position.Z);

            #region Teclas

            if (Input.keyPressed(Key.F))
                showDebugInfo = !showDebugInfo;

            if (Input.keyPressed(Key.E) && CameraInRoom())
                camera.TeleportCamera(Constants.OUTSIDE_SHIP_POSITION);

            if (Input.keyPressed(Key.E) && CameraOutRoom())
                camera.TeleportCamera(Constants.INSIDE_SHIP_POSITION);

            #endregion
        }

        public override void Render()
        {
            PreRender();

            #region Texto en pantalla

            time += ElapsedTime;

            if (showDebugInfo)
            {
                DrawText.drawText("DATOS DE LA Camera: ", 0, 30, Color.Red);
                DrawText.drawText("Posicion: [" + camera.Position.X.ToString() + "; "
                                                + camera.Position.Y.ToString() + "; "
                                                + camera.Position.Z.ToString() + "] ",
                                  0, 60, Color.Red);
                DrawText.drawText("Objetivo: [" + camera.LookAt.X.ToString() + "; "
                                                + camera.LookAt.Y.ToString() + "; "
                                                + camera.LookAt.Z.ToString() + "] ",
                                  0, 80, Color.Red);

                // INFO: Con este nuevo core el elapsedTime hace cualquiera y por ende el TIME no sirve.
                DrawText.drawText("TIME: [" + time.ToString() + "]", 0, 100, Color.Red);

                DrawText.drawText("DATOS DEL AREA ACTUAL: ", 0, 130, Color.Red);

                DrawText.drawText("RANGO DE X: " +
                                    "\nMinimo " + currentCameraArea.xMin.ToString() +
                                    "\nMaximo " + currentCameraArea.xMax.ToString() + "\n\n" +

                                  "RANGO DE Z: " +
                                    "\nMinimo " + currentCameraArea.zMin.ToString() +
                                    "\nMaximo " + currentCameraArea.zMax.ToString(),
                             0, 160, Color.Red);
            }

            #endregion

            #region Renderizado

            physicalworld.Render();

            if (camera.position.Y > 0)
            {
                skyBox.Render();

                if (inSkyBox())
                {
                    water.Render();
                    corales.ForEach(coral => coral.Render());
                    minerals.ForEach(ore => ore.Render());
                    fishes.ForEach(fish => fish.Render());
                    vegetation.ForEach(vegetation => vegetation.Render());
                }
            }
            #endregion

            PostRender();
        }

        private bool inSkyBox()
        {
            return camera.position.X < skyBox.perimeter.xMax && camera.position.X > skyBox.perimeter.xMin &&
                   camera.position.Z < skyBox.perimeter.zMax && camera.position.Z > skyBox.perimeter.zMin;
        }

        public override void Dispose()
        {
            #region Liberacion de recursos
            physicalworld.Dispose();
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
            var treeCorals = meshBuilder.CreateNewScaledMeshes(MeshType.treeCoral, 30, 10);
            meshBuilder.LocateMeshesInTerrain(ref treeCorals, terrain.SizeWorld(), terrain.world);
            var spiralCorals = meshBuilder.CreateNewScaledMeshes(MeshType.spiralCoral, 30, 10);
            meshBuilder.LocateMeshesInTerrain(ref spiralCorals, terrain.SizeWorld(), terrain.world);
            var goldOre = meshBuilder.CreateNewScaledMeshes(MeshType.goldOre, 30, 5);
            meshBuilder.LocateMeshesInTerrain(ref goldOre, terrain.SizeWorld(), terrain.world);
            var silverOre = meshBuilder.CreateNewScaledMeshes(MeshType.silverOre, 30, 5);
            meshBuilder.LocateMeshesInTerrain(ref silverOre, terrain.SizeWorld(), terrain.world);
            var ironOre = meshBuilder.CreateNewScaledMeshes(MeshType.ironOre, 30, 5);
            meshBuilder.LocateMeshesInTerrain(ref ironOre, terrain.SizeWorld(), terrain.world);
            var rock = meshBuilder.CreateNewScaledMeshes(MeshType.rock, 30, 8);
            meshBuilder.LocateMeshesInTerrain(ref rock, terrain.SizeWorld(), terrain.world);
            var alga = meshBuilder.CreateNewScaledMeshes(MeshType.alga, 460, 5);
            meshBuilder.LocateMeshesInTerrain(ref alga, terrain.SizeWorld(), terrain.world);
            var normalFish = meshBuilder.CreateNewScaledMeshes(MeshType.normalFish, 30, 5);
            meshBuilder.LocateMeshesUpToTerrain(ref normalFish, terrain.SizeWorld(), terrain.world, water.world.Center.Y - 200);
            var yellowFish = meshBuilder.CreateNewScaledMeshes(MeshType.yellowFish, 30, 5);
            meshBuilder.LocateMeshesUpToTerrain(ref yellowFish, terrain.SizeWorld(), terrain.world, water.world.Center.Y - 200);

            corales.AddRange(treeCorals);
            corales.AddRange(spiralCorals);
            minerals.AddRange(goldOre);
            minerals.AddRange(silverOre);
            minerals.AddRange(ironOre);
            minerals.AddRange(rock);
            vegetation.AddRange(alga);
            fishes.AddRange(normalFish);
            fishes.AddRange(yellowFish);
        }

        // TODO: Estos dos metodos hay que cambiarlos para calcular distancias con un TGCRay

        private bool CameraInRoom()
        {
            float delta = 300;
            return ship.IndoorMesh.Position.Y - delta < camera.position.Y &&
                   camera.position.Y < ship.IndoorMesh.Position.Y + delta;
        }

        private bool CameraOutRoom()
        {
            var delta = 50;
            return ship.OutdoorMesh.Position.Y - delta < camera.position.Y &&
                 camera.position.Y < ship.OutdoorMesh.Position.Y + delta;

        }

        #endregion
    }
}
