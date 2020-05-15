using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.MeshBuilders;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Watercraft;
using TGC.Group.Utils;
using TGC.Core.Collision;
using static TGC.Group.Model.Terrains.Terrain;
using TGC.Core.Input;
using TGC.Group.Model.Bullet;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        #region Atributos
        private float time;
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
        public struct Perimeter
        {
            public float xMin, xMax, zMin, zMax;

            public Perimeter(float xMin, float xMax, float zMin, float zMax)
            {
                this.xMin = xMin;
                this.xMax = xMax;
                this.zMin = zMin;
                this.zMax = zMax;
            }
        }
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
            terrain = new Terrain(MediaDir, ShadersDir);
            water = new Water(MediaDir, ShadersDir);
            skyBox = new Sky(MediaDir, ShadersDir, camera);
            #endregion

            #region Meshes
            ship = new Ship(MediaDir, ShadersDir);
            shark = new Shark(MediaDir, ShadersDir);
            var Meshes = meshInitializer(skyBox.currentPerimeter); // TODO: Ya que disminui la cantidad, hago que coloque los meshes dentro del skybox asi los vemos, luego cambiar por terrain.SizeWorld().
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
            skyBox.Update();
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
            }
            #endregion

            #region Renderizado
            if (camera.isOutside())
            {
                skyBox.Render();
                water.Render();
                vegetation.ForEach(vegetation => { if (skyBox.Contains(vegetation)) vegetation.Render(); } );
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
            rigidBodyManager.Dispose();
            #endregion
        }

        #region Metodos Privados
        private List<TgcMesh> meshInitializer(Perimeter perimeter)
        {
            #region Ubicar meshes
            var Meshes = new List<TgcMesh>();
            MeshDuplicator.InitOriginalMeshes();

            Meshes.AddRange(createMesh(MeshType.normalCoral, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.treeCoral, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.spiralCoral, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.goldOre, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.silverOre, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.ironOre, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.rock, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.normalFish, 5, perimeter));
            Meshes.AddRange(createMesh(MeshType.yellowFish, 5, perimeter));
            vegetation.AddRange(createMesh(MeshType.alga, 5, perimeter));
            vegetation.AddRange(createMesh(MeshType.alga_2, 5, perimeter));
            vegetation.AddRange(createMesh(MeshType.alga_3, 5, perimeter));
            vegetation.AddRange(createMesh(MeshType.alga_4, 5, perimeter));
            #endregion 
            
            return Meshes;
        }

        private List<TgcMesh> createMesh(MeshType type, int quantity, Perimeter perimeter)
        {
            var meshes = meshBuilder.CreateNewScaledMeshes(type, quantity);
            meshBuilder.LocateMeshesInWorld(type, ref meshes, perimeter, terrain.world, water.world);
            return meshes;
        }

        #endregion
    }
}