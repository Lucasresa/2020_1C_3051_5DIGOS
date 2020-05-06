using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Bullet;
using TGC.Group.Model.MeshBuilders;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Watercraft;
using TGC.Group.Utils;
using TGC.Core.Collision;
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
        private Perimeter currentCameraArea;
        private List<TgcMesh> vegetation = new List<TgcMesh>();
        private List<TgcMesh> Meshes = new List<TgcMesh>();
        private Sky skyBox;
        private Ship ship;
        private Terrain terrain;
        private Water water;
        private Shark shark;
        private MeshBuilder meshBuilder;
        private bool showDebugInfo { get; set; }
        private CameraFPS camera;
        private readonly PhysicalWorld physicalworld = PhysicalWorld.Instance;

        private RigidBody rigidBody = new RigidBody();
        private TgcPickingRay pickingRay;
        private List<TgcMesh> inventory = new List<TgcMesh>();
        private bool showInventory { get; set; }
        private bool showEnterShipInfo { get; set; }
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
            Camera = new CameraFPS(Input, Constants.INSIDE_SHIP_POSITION, Constants.OUTSIDE_SHIP_POSITION);
            camera = (CameraFPS)Camera;            
            #endregion

            #region Mundo            
            terrain = new Terrain(MediaDir, ShadersDir, TGCVector3.Empty);
            water = new Water(MediaDir, ShadersDir, new TGCVector3(0, Constants.WATER_HEIGHT, 0));
            skyBox = new Sky(MediaDir, ShadersDir, camera);
            #endregion

            #region Meshes
            ship = new Ship(MediaDir, ShadersDir);
            shark = new Shark(MediaDir, ShadersDir);
            
            MeshDuplicator.InitOriginalMeshes();
            meshInitializer();
            #endregion

            #region Mundo fisico
            rigidBody.Initializer(terrain, camera, shark, ship, Meshes);
            physicalworld.addInitialRigidBodies(rigidBody.getListRigidBody());
            #endregion

            #region PickingRay
            pickingRay = new TgcPickingRay(Input);
            #endregion

        }

        public override void Update()
        {
            currentCameraArea = terrain.getArea(camera.position.X, camera.position.Z);

            physicalworld.Update(Input, ElapsedTime, TimeBetweenUpdates);

            #region Teclas

            if (Input.keyPressed(Key.F))
                showDebugInfo = !showDebugInfo;
            
            if (characterNearShip())
                showEnterShipInfo = !showEnterShipInfo;
            

            /*
            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                bool selected;
                pickingRay.updateRay();
                foreach (var mineral in Meshes)
                {

                    var aabb = mineral.BoundingBox;

                    //Ejecutar test, si devuelve true se carga el punto de colision collisionPoint
                    selected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out collisionPoint);
                    if (selected && Math.Sqrt(TGCVector3.LengthSq(camera.Position, collisionPoint)) < 500)
                    {
                        inventory.Add(mineral);
                        Meshes.Remove(mineral);
                        break;
                    }


                }

            }*/

            if (Input.keyPressed(Key.J))
                showInventory = !showInventory;


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

            if (showEnterShipInfo)
                DrawText.drawText("PRESIONA E PARA ENTRAR A LA NAVE", 500, 400, Color.White);

            if (showInventory)
                DrawText.drawText("INVENTARIO:" + inventory.Count, 500, 300, Color.White);

            #endregion

            #region Renderizado

            if (camera.position.Y > 0)
            {
                skyBox.Render();
                water.Render();
                vegetation.ForEach(vegetation => {
                    if (inSkyBox(vegetation))
                    {
                        vegetation.AlphaBlendEnable = true;
                        vegetation.Render();
                    } });
            }

            rigidBody.getListRigidBody().ForEach( rigidBody => { if ( inSkyBox(rigidBody) ) rigidBody.Render(); });

            #endregion

            PostRender();
        }

        public override void Dispose()
        {
            #region Liberacion de recursos
            physicalworld.Dispose();
            water.Dispose();
            skyBox.Dispose();
            vegetation.ForEach(vegetation => vegetation.Dispose());
            #endregion
        }

        #region Metodos Privados
        private void meshInitializer()
        {
            #region Ubicar meshes
            var normalCorals = meshBuilder.CreateNewScaledMeshes(MeshType.normalCoral, 100);
            meshBuilder.LocateMeshesInTerrain(ref normalCorals, terrain.SizeWorld(), terrain.world);
            var treeCorals = meshBuilder.CreateNewScaledMeshes(MeshType.treeCoral, 100);
            meshBuilder.LocateMeshesInTerrain(ref treeCorals, terrain.SizeWorld(), terrain.world);
            var spiralCorals = meshBuilder.CreateNewScaledMeshes(MeshType.spiralCoral, 100);
            meshBuilder.LocateMeshesInTerrain(ref spiralCorals, terrain.SizeWorld(), terrain.world);
            var goldOre = meshBuilder.CreateNewScaledMeshes(MeshType.goldOre, 100);
            meshBuilder.LocateMeshesInTerrain(ref goldOre, terrain.SizeWorld(), terrain.world);
            var silverOre = meshBuilder.CreateNewScaledMeshes(MeshType.silverOre, 100);
            meshBuilder.LocateMeshesInTerrain(ref silverOre, terrain.SizeWorld(), terrain.world);
            var ironOre = meshBuilder.CreateNewScaledMeshes(MeshType.ironOre, 100);
            meshBuilder.LocateMeshesInTerrain(ref ironOre, terrain.SizeWorld(), terrain.world);
            var rock = meshBuilder.CreateNewScaledMeshes(MeshType.rock, 100);
            meshBuilder.LocateMeshesInTerrain(ref rock, terrain.SizeWorld(), terrain.world);
            var normalFish = meshBuilder.CreateNewScaledMeshes(MeshType.normalFish, 100);
            meshBuilder.LocateMeshesUpToTerrain(ref normalFish, terrain.SizeWorld(), terrain.world, water.world.Center.Y - 200);
            var yellowFish = meshBuilder.CreateNewScaledMeshes(MeshType.yellowFish, 100);
            meshBuilder.LocateMeshesUpToTerrain(ref yellowFish, terrain.SizeWorld(), terrain.world, water.world.Center.Y - 200);
            var alga = meshBuilder.CreateNewScaledMeshes(MeshType.alga, 200);
            meshBuilder.LocateMeshesInTerrain(ref alga, terrain.SizeWorld(), terrain.world);
            var alga_2 = meshBuilder.CreateNewScaledMeshes(MeshType.alga_2, 200);
            meshBuilder.LocateMeshesInTerrain(ref alga_2, terrain.SizeWorld(), terrain.world);
            var alga_3 = meshBuilder.CreateNewScaledMeshes(MeshType.alga_3, 200);
            meshBuilder.LocateMeshesInTerrain(ref alga_3, terrain.SizeWorld(), terrain.world);
            var alga_4 = meshBuilder.CreateNewScaledMeshes(MeshType.alga_4, 200);
            meshBuilder.LocateMeshesInTerrain(ref alga_4, terrain.SizeWorld(), terrain.world);
            #endregion

            #region Agregar meshes
            vegetation.AddRange(alga);
            vegetation.AddRange(alga_2);
            vegetation.AddRange(alga_3);
            vegetation.AddRange(alga_4);
            Meshes.AddRange(normalCorals);
            Meshes.AddRange(treeCorals);
            Meshes.AddRange(spiralCorals);
            Meshes.AddRange(goldOre);
            Meshes.AddRange(silverOre);
            Meshes.AddRange(ironOre);
            Meshes.AddRange(rock);
            Meshes.AddRange(normalFish);
            Meshes.AddRange(yellowFish);
            #endregion
        }

        private bool inSkyBox(TgcMesh vegetation)
        {
            var posX = vegetation.Position.X;
            var posZ = vegetation.Position.Z;
            return (posX < skyBox.perimeter.xMax && posX > skyBox.perimeter.xMin &&
                     posZ < skyBox.perimeter.zMax && posZ > skyBox.perimeter.zMin);
        }

        private bool inSkyBox(RigidBody rigidBody)
        {
            if (rigidBody.isTerrain || rigidBody.isIndoorShip)
                return true;
            
            var posX = rigidBody.body.CenterOfMassPosition.X;
            var posZ = rigidBody.body.CenterOfMassPosition.Z;
            return (posX < skyBox.perimeter.xMax && posX > skyBox.perimeter.xMin &&
                     posZ < skyBox.perimeter.zMax && posZ > skyBox.perimeter.zMin);
        }

        private bool characterNearShip()
        {
            var character = rigidBody.getListRigidBody()[1];

            pickingRay.updateRay();

            var parte1 = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, ship.OutdoorMesh.BoundingBox, out TGCVector3 collisionPoint);

            pickingRay.updateRay();

            var parte2 = Math.Sqrt(TGCVector3.LengthSq(new TGCVector3(character.body.CenterOfMassPosition), collisionPoint)) < 500;

            return parte1 && parte2;
        }

        #endregion
    }
}
