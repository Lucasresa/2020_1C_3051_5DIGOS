using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.SceneLoader;
using TGC.Group.Model.MeshBuilders;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Watercraft;
using TGC.Group.Utils;
using TGC.Group.Model.Bullet;
using Text = TGC.Group.Model.Draw.Sprite;
using TGC.Core.Text;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        #region Atributos
        private float time;
        private List<TgcMesh> vegetation = new List<TgcMesh>();
        private List<FishMesh> fishes = new List<FishMesh>();
        private CameraFPS camera;
        private Terrain terrain;
        private Water water;
        private Sky skyBox;
        private Ship ship;
        private Shark shark;
        private MeshBuilder meshBuilder;
        private RigidBodyManager rigidBodyManager;
        private Text textInfo = new Text();
        private bool showDebugInfo { get; set; }
        private bool showHelp { get; set; } = true;
        public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);
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
            D3DDevice.Instance.ZFarPlaneDistance = 12000f;
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
            rigidBodyManager.Init(Input,terrain, camera, shark, ship, skyBox, ref Meshes, fishes);
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
                DrawText.drawText("DATOS DE LA Camera: ", 0, 230, Color.Red);
                DrawText.drawText("Posicion: [" + camera.Position.X.ToString() + "; "
                                                + camera.Position.Y.ToString() + "; "
                                                + camera.Position.Z.ToString() + "] ",
                                  0, 260, Color.Red);
                DrawText.drawText("Objetivo: [" + camera.LookAt.X.ToString() + "; "
                                                + camera.LookAt.Y.ToString() + "; "
                                                + camera.LookAt.Z.ToString() + "] ",
                                  0, 280, Color.Red);
		            DrawText.drawText("TIME: [" + time.ToString() + "]", 0, 300, Color.Red);
            }
            if (Input.keyPressed(Key.F1))
                showHelp = !showHelp;

            if (showHelp)
            {
                var text = "Movimiento: W (↑) | A(←) | S(↓) | D(→) " +
                           "\nInstrucciones para salir de la nave: " +
                           "\n\tPara salir de la nave mirar hacia la escotilla y presionar la tecla E" +
                           "\nRecolectar y atacar: " +
                           "\n\tPara recolectar los objetos acercarse y clickearlos." +
                           "\n\tPara atacar al tiburon hacer click derecho cuando se tiene el arma." +
                           "\n\tUna vez crafteada el arma se equipa con el numero 1 y para desequiparla presionar el 0" +
                           "\nCrafteos dentro de la nave: " +
                           "\n\tArma: necesitas recolectar dos rocas y dos metales de plata y apretar la tecla M" +
                           "\n\tRed para agarrar peces: necesitas recolectar 1 coral de cada tipo y un metal de hierro y apretar la tecla N" +
                           "\n\tCasco de oxigeno: necesitas recolectar 4 metales de oro y apretar dentro de la nave la tecla B" +
                           "\nPara abrir y cerrar la ayuda apretar F1";

                (int width, int height) size = (width: 1200, height: 600);
                (int posX, int posY) position = (posX: 10, posY: (screen.height - size.height) / 2);
                textInfo.drawText(text, Color.HotPink, new Point(position.posX, position.posY), new Size(size.width, size.height), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 12, FontStyle.Bold));
            }

            #endregion

            #region Renderizado
            if (camera.isOutside())
            {
                skyBox.Render(terrain.SizeWorld());
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
            textInfo.Dispose();
            #endregion
        }

        #region Metodos Privados
        private List<TgcMesh> meshInitializer(Perimeter perimeter)
        {
            #region Ubicar meshes
            var Meshes = new List<TgcMesh>();
            MeshDuplicator.InitOriginalMeshes();

            var terrainSize = terrain.SizeWorld();

            Meshes.AddRange(createMesh(MeshType.normalCoral, 30, terrainSize));
            Meshes.AddRange(createMesh(MeshType.treeCoral, 30, terrainSize));
            Meshes.AddRange(createMesh(MeshType.spiralCoral, 30, terrainSize));
            Meshes.AddRange(createMesh(MeshType.goldOre, 30, terrainSize));
            Meshes.AddRange(createMesh(MeshType.silverOre, 30, terrainSize));
            Meshes.AddRange(createMesh(MeshType.ironOre, 30, terrainSize));
            Meshes.AddRange(createMesh(MeshType.rock, 30, terrainSize));
            vegetation.AddRange(createMesh(MeshType.alga, 40, perimeter));
            vegetation.AddRange(createMesh(MeshType.alga_2, 20, perimeter));
            vegetation.AddRange(createMesh(MeshType.alga_3, 20, perimeter));
            vegetation.AddRange(createMesh(MeshType.alga_4, 20, perimeter));

            var normalFishes = createMesh(MeshType.normalFish, 16, perimeter);
            var yellowFishes = createMesh(MeshType.yellowFish, 8, perimeter);
            fishes.AddRange(normalFishes.ConvertAll(fish => new FishMesh(fish, skyBox, terrain)));
            fishes.AddRange(yellowFishes.ConvertAll(fish => new FishMesh(fish, skyBox, terrain)));
            fishes.ForEach(fish => fish.Init());
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