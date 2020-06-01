using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
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
        private FullQuad FullQuad;
        
        private bool ActiveInventory { get; set; }
        private bool CanCraftObjects => ObjectManager.Character.IsInsideShip;

        public float Time { get; set; }
        public float TimeToRevive { get; set; }
        public float TimeToAlarm { get; set; }
        public float ItemHistoryTime { get; set; }

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) => FixedTickEnable = true;

        public override void Init()
        {
            Camera = new CameraFPS(Input);
            camera = (CameraFPS)Camera;
            FullQuad = new FullQuad(MediaDir, ShadersDir, ElapsedTime);
            FullQuad.Initializer("PostProcess");
            Ray = new Ray(Input);
            ObjectManager = new GameObjectManager(MediaDir, ShadersDir, camera, Input, Ray);
            CharacterStatus = new CharacterStatus(ObjectManager.Character);
            SharkStatus = new SharkStatus();
            Draw2DManager = new Game2DManager(MediaDir, CharacterStatus, SharkStatus);
            EventsManager = new GameEventsManager(ObjectManager.Shark, ObjectManager.Character);
            InventoryManager = new GameInventoryManager();
        }

        public override void Update()
        {
            Time += ElapsedTime;
                        
            if (Input.keyDown(Key.F2)) D3DDevice.Instance.Device.RenderState.FillMode = FillMode.WireFrame;
            else D3DDevice.Instance.Device.RenderState.FillMode = FillMode.Solid;

            if (Input.keyPressed(Key.F1)) Draw2DManager.ShowHelp = !Draw2DManager.ShowHelp;

            if (CharacterStatus.IsDead)
            {
                TimeToRevive += ElapsedTime;
                if (TimeToRevive < 5)
                {
                    FullQuad.SetTime(ElapsedTime);
                    FullQuad.RenderTeleportEffect = true;
                }
                else
                {
                    CharacterStatus.Respawn();
                    FullQuad.RenderTeleportEffect = FullQuad.RenderAlarmEffect = false;
                }
                return;
            }

            TimeToRevive = 0;

            if (Input.keyPressed(Key.I)) Draw2DManager.ActiveInventory = camera.Lock =
                    FullQuad.RenderPDA = ActiveInventory = !ActiveInventory;

            if (!ActiveInventory)
            {
                ObjectManager.UpdateCharacter();
                ObjectManager.Update(ElapsedTime, TimeBetweenUpdates);
                EventsManager.Update(ElapsedTime, ObjectManager.Fishes);
                InventoryManager.AddItem(ObjectManager.ItemSelected);
                Draw2DManager.ItemHistory = InventoryManager.ItemHistory;
                ObjectManager.ItemSelected = null;
                CharacterStatus.DamageReceived = ObjectManager.Shark.AttackedCharacter;
                CharacterStatus.Update(ElapsedTime);
                FullQuad.RenderAlarmEffect = CharacterStatus.ActiveRenderAlarm;
                Draw2DManager.DistanceWithShip = FastUtils.DistanceBetweenVectors(camera.Position, ObjectManager.Ship.PositionShip);
                Draw2DManager.ShowIndicatorShip = Draw2DManager.DistanceWithShip > 15000 && !ObjectManager.Character.IsInsideShip;
                if (CharacterStatus.ActiveAlarmForDamageReceived)
                {
                    TimeToAlarm += ElapsedTime;
                    if (TimeToAlarm > 2)
                    {
                        FullQuad.RenderAlarmEffect = CharacterStatus.ActiveAlarmForDamageReceived = false;
                        TimeToAlarm = 0;
                    }
                }

                ObjectManager.Shark.AttackedCharacter = CharacterStatus.DamageReceived;
                SharkStatus.DamageReceived = ObjectManager.Character.AttackedShark;
                SharkStatus.Update();
                ObjectManager.Character.AttackedShark = SharkStatus.DamageReceived;
                Draw2DManager.Update();
                Draw2DManager.Inventory.UpdateItems(InventoryManager.Items);
            }            

            if (Input.keyPressed(Key.E)) ObjectManager.Character.Teleport();

            if (CanCraftObjects)
            {
                if (Input.keyPressed(Key.M)) ObjectManager.Character.HasWeapon = GameCraftingManager.CanCraftWeapon(InventoryManager.Items);
                if (Input.keyPressed(Key.N)) ObjectManager.Character.HasDivingHelmet = CharacterStatus.HasDivingHelmet = GameCraftingManager.CanCraftDivingHelmet(InventoryManager.Items);
                if (Input.keyPressed(Key.B)) ObjectManager.Character.CanFish = GameCraftingManager.CanCatchFish(InventoryManager.Items);
            }

            Draw2DManager.ShowInfoExitShip = ObjectManager.Character.LooksAtTheHatch;
            Draw2DManager.ShowInfoEnterShip = ObjectManager.Character.NearShip;
            Draw2DManager.NearObjectForSelect = ObjectManager.NearObjectForSelect;
            Draw2DManager.ShowInfoItemCollect = ObjectManager.ShowInfoItemCollect;

            UpdateInfoItemCollect();

            if (Input.keyPressed(Key.P))
                ObjectManager.Character.CanFish = ObjectManager.Character.HasWeapon =
                    ObjectManager.Character.HasDivingHelmet = true;
        }

        private void UpdateInfoItemCollect()
        {
            if (!Draw2DManager.ShowInfoItemCollect) return;
            
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
            FullQuad.PreRenderMeshes();
            ObjectManager.Render();
            FullQuad.Render();
            Draw2DManager.Render();
            PostRender();
        }

        public override void Dispose()
        {
            FullQuad.Dispose();
            ObjectManager.Dispose();
            Draw2DManager.Dispose();
        }
    }
}