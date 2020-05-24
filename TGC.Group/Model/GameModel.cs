using Microsoft.DirectX.DirectInput;
using System.Security.Policy;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Group.Model.Objects;
using TGC.Group.Model.Status;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
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

        private CameraFPS camera;
        private GameObjectManager ObjectManager;
        private Game2DManager Draw2DManager;
        private GameEventsManager EventsManager;
        private CharacterStatus CharacterStatus;
        private SharkStatus SharkStatus;
        private Ray Ray;
        private GameInventoryManager InventoryManager;
        
        private bool ActiveInventory { get; set; }

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FixedTickEnable = true;
            D3DDevice.Instance.ZFarPlaneDistance = 12000f;
        }

        public override void Init()
        {
            Camera = new CameraFPS(Input);
            camera = (CameraFPS)Camera;
            Ray = new Ray(Input);
            ObjectManager = new GameObjectManager(MediaDir, ShadersDir, camera, Input, Ray);
            CharacterStatus = new CharacterStatus(ObjectManager.Character);
            SharkStatus = new SharkStatus(ObjectManager.Shark);
            Draw2DManager = new Game2DManager(MediaDir, CharacterStatus, SharkStatus);
            EventsManager = new GameEventsManager(ObjectManager.Shark, ObjectManager.Character);
            InventoryManager = new GameInventoryManager();
        }

        public override void Update()
        {
            ObjectManager.UpdateCharacter();

            if (Input.keyPressed(Key.I))
                Draw2DManager.ActiveInventory = camera.Lock = ActiveInventory = !ActiveInventory;

            if (!ActiveInventory)
            {
                ObjectManager.Update(ElapsedTime, TimeBetweenUpdates);
                EventsManager.Update(ElapsedTime, ObjectManager.Fish);
                InventoryManager.AddItem(ObjectManager.ItemSelected);
                ObjectManager.ItemSelected = (0, null);
                CharacterStatus.Update();
                SharkStatus.Update();
                Draw2DManager.Update();
                Draw2DManager.UpdateItems(InventoryManager.ListItems);
            }

            if (Input.keyPressed(Key.E))
                ObjectManager.Character.Teleport();

        }

        public override void Render()
        {
            PreRender();
            ObjectManager.Render();
            Draw2DManager.Render();
            PostRender();
        }

        public override void Dispose()
        {
            ObjectManager.Dispose();
            Draw2DManager.Dispose();
        }
    }
}