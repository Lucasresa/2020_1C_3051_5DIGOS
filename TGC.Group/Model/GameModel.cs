using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
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
        private FullQuad FullQuad;
        private GameObjectManager ObjectManager;
        private GameInventoryManager InventoryManager;
        private GameEventsManager EventsManager;
        private Game2DManager Draw2DManager;
        private CharacterStatus CharacterStatus;
        private SharkStatus SharkStatus;
        
        private bool ActiveInventory { get; set; }
        private bool CanCraftObjects => ObjectManager.Character.IsInsideShip;

        public float TimeToRevive { get; set; }
        public float TimeToAlarm { get; set; }
        public float ItemHistoryTime { get; set; }

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir) => FixedTickEnable = true;

        public override void Init()
        {
            Camera = camera = new CameraFPS(Input);
            FullQuad = new FullQuad(MediaDir, ShadersDir, ElapsedTime);            
            ObjectManager = new GameObjectManager(MediaDir, ShadersDir, camera, Input);
            CharacterStatus = new CharacterStatus(ObjectManager.Character);
            SharkStatus = new SharkStatus();
            Draw2DManager = new Game2DManager(MediaDir, CharacterStatus, SharkStatus);
            EventsManager = new GameEventsManager(ObjectManager.Shark, ObjectManager.Character);
            InventoryManager = new GameInventoryManager();
        }

        public override void Update()
        {
            if (Input.keyPressed(Key.F1)) Draw2DManager.ShowHelp = !Draw2DManager.ShowHelp;

            if (CharacterStatus.IsDead)
                UpdateCharacterIsDead();
            TimeToRevive = 0;

            if (Input.keyPressed(Key.I)) Draw2DManager.ActiveInventory = camera.Lock =
                    FullQuad.RenderPDA = ActiveInventory = !ActiveInventory;

            if (!ActiveInventory)
                UpdateGame();

            if (Input.keyPressed(Key.E)) ObjectManager.Character.Teleport();

            UpdateFlags();

            UpdateInfoItemCollect();

            if (Input.keyPressed(Key.P))
                ObjectManager.Character.CanFish = ObjectManager.Character.HasWeapon =
                    ObjectManager.Character.HasDivingHelmet = true;
        }

        private void UpdateGame()
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

        private void UpdateCharacterIsDead()
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

        private void UpdateFlags()
        {
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