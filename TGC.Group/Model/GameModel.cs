using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
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
        private GameInventoryManager InventoryManager;
        private GameEventsManager EventsManager;
        private Game2DManager Draw2DManager;
        private CharacterStatus CharacterStatus;
        private SharkStatus SharkStatus;
        private Ray Ray;
        
        private bool ActiveInventory { get; set; }
        private bool CanCraftObjects { get { return ObjectManager.Character.IsInsideShip; } }

        public float Time { get; set; }
        public float ItemHistoryTime { get; set; }

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
            Time += ElapsedTime;

            if (Input.keyPressed(Key.I))
                Draw2DManager.ActiveInventory = camera.Lock = ActiveInventory = !ActiveInventory;

            if (!ActiveInventory)
            {
                ObjectManager.UpdateCharacter();
                ObjectManager.Update(ElapsedTime, TimeBetweenUpdates);
                EventsManager.Update(ElapsedTime, ObjectManager.Fish);
                InventoryManager.AddItem(ObjectManager.ItemSelected);
                Draw2DManager.ItemHistory = InventoryManager.ItemHistory;
                ObjectManager.ItemSelected = null;
                CharacterStatus.Update();
                SharkStatus.Update();
                Draw2DManager.Update();
                Draw2DManager.UpdateItems(InventoryManager.Items);
            }

            if (Input.keyPressed(Key.E))
                ObjectManager.Character.Teleport();

            if (CanCraftObjects)
            {
                if (Input.keyPressed(Key.M))
                    ObjectManager.Character.HasWeapon = GameCraftingManager.CanCraftWeapon(InventoryManager.Items);
                if (Input.keyPressed(Key.N))
                    ObjectManager.Character.HasDivingHelmet = CharacterStatus.HasDivingHelmet = GameCraftingManager.CanCraftDivingHelmet(InventoryManager.Items);
                if (Input.keyPressed(Key.B))
                    ObjectManager.Character.CanFish = GameCraftingManager.CanCatchFish(InventoryManager.Items);
            }

            if (Input.keyPressed(Key.F1))
                Draw2DManager.ShowHelp = !Draw2DManager.ShowHelp;

            Draw2DManager.ShowInfoExitShip = ObjectManager.Character.LooksAtTheHatch;
            Draw2DManager.ShowInfoEnterShip = ObjectManager.Character.NearShip;
            Draw2DManager.NearObjectForSelect = ObjectManager.NearObjectForSelect;
            Draw2DManager.ShowInfoItemCollect = ObjectManager.ShowInfoItemCollect;

            UpdateInfoItemCollect();
        }

        private void UpdateInfoItemCollect()
        {
            if (!Draw2DManager.ShowInfoItemCollect)
                return;
            
            ItemHistoryTime += ElapsedTime;
            if (ItemHistoryTime > Draw2DManager.ItemHistoryTime)
            {
                Draw2DManager.ShowInfoItemCollect = false;
                InventoryManager.ItemHistory.RemoveRange(0, InventoryManager.ItemHistory.Count);
                ItemHistoryTime = 0;
            }
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