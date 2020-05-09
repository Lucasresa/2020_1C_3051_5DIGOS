using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
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
using static TGC.Group.Model.Terrains.Terrain;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    { 
        #region Atributos
        private float time;
        private Perimeter currentCameraArea;
        private List<TgcMesh> vegetation = new List<TgcMesh>();
        private CameraFPS camera;
        private Terrain terrain;
        private Water water;
        private Sky skyBox;
        private Ship ship;
        private Shark shark;
        private MeshBuilder meshBuilder;
        private RigidBodyManager rigidBodyManager;
        private bool showDebugInfo { get; set; }
        #endregion

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            #region Configuracion
            FixedTickEnable = true;
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
            meshBuilder = new MeshBuilder();
            MeshDuplicator.MediaDir = mediaDir;
            D3DDevice.Instance.ZFarPlaneDistance = 8000f;
            #endregion
        }

        public override void Init()
        {
            #region Camera 
            Camera = new CameraFPS(Input);
            camera = (CameraFPS)Camera;            
            #endregion
            
            #region Mundo            
            terrain = new Terrain(MediaDir, ShadersDir, TGCVector3.Empty);
            water = new Water(MediaDir, ShadersDir, TGCVector3.Empty);
            skyBox = new Sky(MediaDir, ShadersDir, camera);
            #endregion

            #region Meshes
            ship = new Ship(MediaDir, ShadersDir);
            shark = new Shark(MediaDir, ShadersDir);                      
            var Meshes = meshInitializer();
            #endregion
                        
            #region Mundo fisico
            rigidBodyManager = new RigidBodyManager(MediaDir, ShadersDir);
            rigidBodyManager.Init(Input,terrain, camera, shark, ship, skyBox, ref Meshes);
            #endregion
        }

        public override void Update()
        {
            #region Update
            rigidBodyManager.Update(Input, ElapsedTime, TimeBetweenUpdates);
            currentCameraArea = terrain.getArea(camera.position.X, camera.position.Z);
            #endregion

            #region Teclas

            if (Input.keyPressed(Key.F))
                showDebugInfo = !showDebugInfo;

            #endregion
        }

        public override void Render()
        {
            PreRender();
            
            #region Texto en pantalla
            time += ElapsedTime;
            
            if (showDebugInfo)
            {   // TODO: AJUSTAR LA POSICION DONDE SE MUESTRE EN PANTALLA LA INFORMACION
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
            #endregion

            #region Renderizado
            if (isOutside())
            {
                skyBox.Render();
                water.Render();
                vegetation.ForEach(vegetation => { if (skyBox.inSkyBox(vegetation)) vegetation.Render(); } );
            }

            rigidBodyManager.Render();
            #endregion

            PostRender();
        }

        public override void Dispose()
        {
            #region Liberacion de recursos
            water.Dispose();
            skyBox.Dispose();
            vegetation.ForEach(vegetation => vegetation.Dispose());
            #endregion
        }

        #region Metodos Privados

        private List<TgcMesh> meshInitializer()
        {
            MeshDuplicator.InitOriginalMeshes();
            List<TgcMesh> Meshes = new List<TgcMesh>();

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
            var alga = meshBuilder.CreateNewScaledMeshes(MeshType.alga, 500);
            meshBuilder.LocateMeshesInTerrain(ref alga, terrain.SizeWorld(), terrain.world);
            var alga_2 = meshBuilder.CreateNewScaledMeshes(MeshType.alga_2, 500);
            meshBuilder.LocateMeshesInTerrain(ref alga_2, terrain.SizeWorld(), terrain.world);
            var alga_3 = meshBuilder.CreateNewScaledMeshes(MeshType.alga_3, 500);
            meshBuilder.LocateMeshesInTerrain(ref alga_3, terrain.SizeWorld(), terrain.world);
            var alga_4 = meshBuilder.CreateNewScaledMeshes(MeshType.alga_4, 500);
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

            return Meshes;
            #endregion
        }

        private bool isOutside()
        {
            return camera.position.Y > 0;
        }

        #endregion
    }
}