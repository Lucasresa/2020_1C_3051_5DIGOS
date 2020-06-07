using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
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

        private GameState StateGame;
        private GameState StateMenu;
        private GameState StateHelp;
        private GameState StateExitHelp;
        private GameState CurrentState;

        private List<DrawButton> Buttons = new List<DrawButton>();

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

        public override void Dispose()
        {
            FullQuad.Dispose();
            ObjectManager.Dispose();
            Draw2DManager.Dispose();
        }

        public override void Render() => CurrentState.Render();

        private void RenderMenu()
        {
            PreRender();
            if (ObjectManager.ShowScene)
            {
                ObjectManager.Skybox.Render();
                ObjectManager.Ship.OutdoorMesh.Render();
                ObjectManager.Water.Render();
            }
            Buttons.ForEach(button => button.Render());
            PostRender();
        }

        private void RenderGame()
        {
            FullQuad.PreRenderMeshes();
            ObjectManager.Render();
            FullQuad.Render();
            Draw2DManager.Render();
            PostRender();
        }

        public override void Init()
        {
            Camera = camera = new CameraFPS(Input);
            FullQuad = new FullQuad(MediaDir, ShadersDir, ElapsedTime);
            Draw2DManager = new Game2DManager(MediaDir, CharacterStatus, SharkStatus);
            InitializerState();

            InitializerMenu();
            InitializerGame();
        }
        
        private void InitializerMenu()
        {
            var play = new DrawButton(MediaDir, Input);
            play.InitializerButton(text: "Play", scale: new TGCVector2(0.4f, 0.4f), position: new TGCVector2(20, 500),
                           action: () => CurrentState = StateGame);
            var help = new DrawButton(MediaDir, Input);
            help.InitializerButton(text: "Help", scale: new TGCVector2(0.4f, 0.4f), position: new TGCVector2(20, 580),
                           action: () => CurrentState = StateHelp);
            var exitHelp = new DrawButton(MediaDir, Input);
            exitHelp.InitializerButton(text: "Exit", scale: new TGCVector2(0.4f, 0.4f), 
                                   position: new TGCVector2(Draw2DManager.ScreenWitdh - help.Size.X - 50, Draw2DManager.ScreenHeight - help.Size.Y - 50),
                                   action: () => CurrentState = StateExitHelp);
            Buttons.Add(play);
            Buttons.Add(help);
            Buttons.Add(exitHelp);

            camera.Position = new TGCVector3(1030, 3900, 2500);
            camera.Lock = true;
        }

        private void InitializerGame()
        {
            ObjectManager = new GameObjectManager(MediaDir, ShadersDir, camera, Input);
            CharacterStatus = new CharacterStatus(ObjectManager.Character);
            SharkStatus = new SharkStatus();
            EventsManager = new GameEventsManager(ObjectManager.Shark, ObjectManager.Character);
            InventoryManager = new GameInventoryManager();
        }

        private void InitializerState()
        {
            StateGame = new GameState()
            {
                Update = UpdateGame,
                Render = RenderGame
            };

            StateMenu = new GameState()
            {
                Update = UpdateMenu,
                Render = RenderMenu
            };

            StateHelp = new GameState()
            {
                Update = UpdateHelp,
                Render = Draw2DManager.RenderHelp
            };

            StateExitHelp = new GameState()
            {
                Update = UpdateExitHelp,
                Render = Draw2DManager.RenderHelp
            };

            CurrentState = StateMenu;
        }

        private void UpdateHelp() => Buttons.ForEach(button => { if (button.ButtonText.Text == "Help") button.Update(); });
        private void UpdateExitHelp() => Buttons.ForEach(button => { if (button.ButtonText.Text == "Exit") button.Update(); });

        private void UpdateMenu()
        {
            Buttons.ForEach(button => button.Update());
            
            if (ObjectManager.ShowScene)
            {
                ObjectManager.Skybox.Update();
                ObjectManager.Water.Update(ElapsedTime);
            }

            if (CurrentState != StateMenu)
            {
                Buttons.ForEach(button => button.Dispose());
                camera.Lock = false;
            }
        }

        public override void Update() => CurrentState.Update();

        #region UpdateGame

        private void UpdateGame()
        {          
            if (Input.keyPressed(Key.F1)) Draw2DManager.ShowHelp = !Draw2DManager.ShowHelp;

            if (CharacterStatus.IsDead)
                UpdateCharacterIsDead();
            TimeToRevive = 0;

            if (Input.keyPressed(Key.I)) Draw2DManager.ActiveInventory = camera.Lock =
                    FullQuad.RenderPDA = ActiveInventory = !ActiveInventory;

            if (!ActiveInventory)
                UpdateEvents();

            if (Input.keyPressed(Key.E)) ObjectManager.Character.Teleport();

            UpdateFlags();

            UpdateInfoItemCollect();

            if (Input.keyPressed(Key.P))
                ObjectManager.Character.CanFish = ObjectManager.Character.HasWeapon =
                    ObjectManager.Character.HasDivingHelmet = true;
        }

        private void UpdateEvents()
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

        #endregion UpdateGame
    }
}