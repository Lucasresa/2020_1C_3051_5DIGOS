using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Security.AccessControl;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.Status;
using TGC.Group.Utils;
using static TGC.Group.Model.GameInventoryManager;

namespace TGC.Group.Model
{
    class Game2DManager
    {
        private struct Constants
        {
            public static int SCREEN_WIDTH = D3DDevice.Instance.Device.Viewport.Width;
            public static int SCREEN_HEIGHT = D3DDevice.Instance.Device.Viewport.Height;
            public static TGCVector2 LIFE_CHARACTER_POSITION = new TGCVector2(100, 0);
            public static TGCVector2 LIFE_CHARACTER_SCALE = new TGCVector2(0.4f, 0.5f);
            public static TGCVector2 LIFE_CHARACTER_TEXT_POSITION = new TGCVector2(10f, 20f);
            public static TGCVector2 OXYGEN_CHARACTER_POSITION = new TGCVector2(100, 30);
            public static TGCVector2 OXYGEN_CHARACTER_SCALE = new TGCVector2(0.4f, 0.5f);
            public static TGCVector2 OXYGEN_CHARACTER_TEXT_POSITION = new TGCVector2(10f, 50f);
            public static TGCVector2 LIFE_SHARK_SCALE = new TGCVector2(0.4f, 0.5f);
            public static TGCVector2 LIFE_SHARK_SIZE = new TGCVector2(1000 * LIFE_SHARK_SCALE.X, 100 * LIFE_SHARK_SCALE.Y);
            public static TGCVector2 LIFE_SHARK_POSITION = new TGCVector2(SCREEN_WIDTH - LIFE_SHARK_SIZE.X - 20, 0);
            public static TGCVector2 LIFE_SHARK_TEXT_SIZE = new TGCVector2(130, 100);
            public static TGCVector2 LIFE_SHARK_TEXT_POSITION = new TGCVector2(LIFE_SHARK_POSITION.X - LIFE_SHARK_TEXT_SIZE.X, 20f);
            public static TGCVector2 POINTER_SCALE = new TGCVector2(1, 1);
            public static TGCVector2 POINTER_SIZE = new TGCVector2(64 * POINTER_SCALE.X, 64 * POINTER_SCALE.Y);
            public static TGCVector2 POINTER_POSITION = new TGCVector2((SCREEN_WIDTH - POINTER_SIZE.X) / 2, (SCREEN_HEIGHT - POINTER_SIZE.Y) /2);
            public static TGCVector2 MOUSE_POINTER_SCALE = new TGCVector2(1, 1);
            public static TGCVector2 MOUSE_POINTER_SIZE = new TGCVector2(32 * POINTER_SCALE.X, 32 * POINTER_SCALE.Y);
            public static TGCVector2 MOUSE_POINTER_POSITION = new TGCVector2((SCREEN_WIDTH - MOUSE_POINTER_SIZE.X) / 2, (SCREEN_HEIGHT - MOUSE_POINTER_SIZE.Y) /2);
            public static TGCVector2 INVENTORY_TEXT_SIZE = new TGCVector2(300, 300);
            public static TGCVector2 INVENTORY_TEXT_POSITION = new TGCVector2(10, SCREEN_HEIGHT - INVENTORY_TEXT_SIZE.Y);
            public static string INVENTORY_TEXT_WITHOUT_ITEMS = "Inventory without items!";
            public static TGCVector2 COMMON_TEXT_SIZE = new TGCVector2(300, 50);
            public static TGCVector2 HELP_TEXT_POSITION = new TGCVector2(SCREEN_WIDTH - COMMON_TEXT_SIZE.X + 30, SCREEN_HEIGHT - COMMON_TEXT_SIZE.Y + 20);
            public static TGCVector2 INSTRUCTION_TEXT_SIZE = new TGCVector2(850, 450);
            public static TGCVector2 INSTRUCTION_TEXT_POSITION = new TGCVector2((SCREEN_WIDTH - INSTRUCTION_TEXT_SIZE.X) / 2, (SCREEN_HEIGHT - INSTRUCTION_TEXT_SIZE.Y) / 2);
            public static string INSTRUCTION_TEXT = "Movement: W(↑) | A(←) | S(↓) | D(→) " +
                                                    "\nInstructions for leaving and entering the ship: " +
                                                    "\n\t-To exit the ship look towards the hatch and press the E key." +
                                                    "\n\t-To enter the ship, come closer and press the E key." +
                                                    "\nCollect and attack: " +
                                                    "\n\t-To collect the objects, left click on them." +
                                                    "\n\tTo attack the shark, right click when you have the weapon." +
                                                    "\n\tOnce the weapon is crafted, it is equipped and unequipped with the number 1." +
                                                    "\nInventory: " +
                                                    "\n\t-To open and close, press I key." +
                                                    "\nCrafting inside the ship: " +
                                                    "\n\t-Weapon: Press the M key." +
                                                    "\n\t-Diving Helmet: Press the B key." +
                                                    "\n\t-Ability to collect fish: ¨Press the N key." +
                                                    "\nTo open and close help, press F1 key.";
            public static TGCVector2 PRESS_TEXT_POSITION = new TGCVector2((SCREEN_WIDTH - COMMON_TEXT_SIZE.X + 145) /2 ,(SCREEN_HEIGHT - COMMON_TEXT_SIZE.Y - 30) /2);
        }
        
        private readonly string MediaDir;
        private readonly CharacterStatus Character;
        private readonly SharkStatus Shark;
        private readonly DrawSprite LifeShark;
        private readonly DrawSprite LifeCharacter;
        private readonly DrawSprite MousePointer;
        private readonly DrawSprite OxygenCharacter;
        private readonly DrawSprite Pointer;
        private readonly DrawText LifeCharacterText;
        private readonly DrawText LifeSharkText;
        private readonly DrawText OxygenCharacterText;
        private readonly DrawText InventoryText;
        private readonly DrawText InstructionText;
        private readonly DrawText CommonText;

        public bool ActiveInventory { get; set; }
        public bool ShowHelp { get; set; }
        public bool ShowInfoExitShip { get; set; }
        public bool ShowInfoEnterShip { get; set; }

        public Game2DManager(string mediaDir, CharacterStatus character, SharkStatus shark)
        {
            MediaDir = mediaDir;
            Character = character;
            Shark = shark;
            LifeCharacter = new DrawSprite(MediaDir);
            LifeShark = new DrawSprite(MediaDir);
            OxygenCharacter = new DrawSprite(MediaDir);
            Pointer = new DrawSprite(MediaDir);
            MousePointer = new DrawSprite(MediaDir);
            LifeCharacterText = new DrawText();
            LifeSharkText = new DrawText();
            OxygenCharacterText = new DrawText();
            InventoryText = new DrawText();
            InstructionText = new DrawText();
            CommonText = new DrawText();
            Init();
        }

        public void Dispose()
        {
            InstructionText.Dispose();
            CommonText.Dispose();
            InventoryText.Dispose();
            LifeCharacter.Dispose();
            LifeCharacterText.Dispose();
            LifeShark.Dispose();
            LifeSharkText.Dispose();
            MousePointer.Dispose();
            OxygenCharacter.Dispose();
            OxygenCharacterText.Dispose();
            Pointer.Dispose();
        }

        private void Init()
        {
            InitializerLifeCharacter();
            InitializerLifeShark();
            InitializerOxygenCharacter();
            InitializerPointer();
            InitializerMousePointer();
            InitializerInventoryText();
            InitializerInstructionText();
        }

        private void InitializerLifeCharacter()
        {
            LifeCharacter.SetImage("LifeBar.png");
            LifeCharacter.SetInitialScallingAndPosition(Constants.LIFE_CHARACTER_SCALE, Constants.LIFE_CHARACTER_POSITION);
            LifeCharacterText.SetTextAndPosition(text: "LIFE", position: Constants.LIFE_CHARACTER_TEXT_POSITION);
            LifeCharacterText.Color = Color.MediumVioletRed;
        }

        private void InitializerLifeShark()
        {
            LifeShark.SetImage("LifeBar.png");
            LifeShark.SetInitialScallingAndPosition(Constants.LIFE_SHARK_SCALE, Constants.LIFE_SHARK_POSITION);
            LifeSharkText.SetTextSizeAndPosition(text: "LIFE SHARK", size: Constants.LIFE_SHARK_TEXT_SIZE, position: Constants.LIFE_SHARK_TEXT_POSITION);
            LifeSharkText.Color = Color.MediumVioletRed;
        }

        private void InitializerOxygenCharacter()
        {
            OxygenCharacter.SetImage("OxygenBar.png");
            OxygenCharacter.SetInitialScallingAndPosition(Constants.OXYGEN_CHARACTER_SCALE, Constants.OXYGEN_CHARACTER_POSITION);
            OxygenCharacterText.SetTextAndPosition(text: "OXYGEN", position: Constants.OXYGEN_CHARACTER_TEXT_POSITION);
            OxygenCharacterText.Color = Color.DeepSkyBlue;
        }
        
        private void InitializerPointer()
        {
            Pointer.SetImage("Pointer.png");
            Pointer.SetInitialScallingAndPosition(Constants.POINTER_SCALE, Constants.POINTER_POSITION);
        }

        private void InitializerMousePointer()
        {
            MousePointer.SetImage("MousePointer.png");
            MousePointer.SetInitialScallingAndPosition(Constants.MOUSE_POINTER_SCALE, Constants.MOUSE_POINTER_POSITION);
        }

        private void InitializerInventoryText()
        {
            InventoryText.SetTextSizeAndPosition(text: Constants.INVENTORY_TEXT_WITHOUT_ITEMS, Constants.INVENTORY_TEXT_SIZE, Constants.INVENTORY_TEXT_POSITION);
            InventoryText.Color = Color.Black;
        }

        private void InitializerInstructionText()
        {
            InstructionText.SetTextSizeAndPosition(text: Constants.INSTRUCTION_TEXT, Constants.INSTRUCTION_TEXT_SIZE, Constants.INSTRUCTION_TEXT_POSITION);
            InstructionText.Color = Color.Red;
        }

        public void Render()
        {
            if (ShowHelp)
                InstructionText.Render();
            else
                CommonText.DrawSimpleText(text: "FOR HELP, PRESS F1 KEY", size: Constants.COMMON_TEXT_SIZE, position: Constants.HELP_TEXT_POSITION, color: Color.Red);

            if (!ActiveInventory)
            {
                LifeShark.Render();
                LifeCharacter.Render();
                OxygenCharacter.Render();
                LifeSharkText.Render();
                LifeCharacterText.Render();
                OxygenCharacterText.Render();
                Pointer.Render();
                if (ShowInfoExitShip)
                    CommonText.DrawSimpleText(text: "PRESS E TO EXIT", size: Constants.COMMON_TEXT_SIZE, position: Constants.PRESS_TEXT_POSITION, color: Color.White);
                if (ShowInfoEnterShip)
                    CommonText.DrawSimpleText(text: "PRESS E TO ENTER", size: Constants.COMMON_TEXT_SIZE, position: Constants.PRESS_TEXT_POSITION, color: Color.White);
            }
            else
            {
                MousePointer.Position = new TGCVector2(Cursor.Position.X - 16 , Cursor.Position.Y - 16 );
                MousePointer.Render();
                InventoryText.Render();
            }
        }

        public void Update()
        {
            UpdateSprite(LifeShark, Shark.Life, Shark.GetLifeMax());
            UpdateSprite(LifeCharacter, Character.Life, Character.GetLifeMax());
            UpdateSprite(OxygenCharacter, Character.Oxygen, Character.GetOxygenMax());
        }

        public void UpdateItems(Dictionary<string, List<string>> items)
        {
            var hasItems = items.Values.ToList().Any(listItems => listItems.Count > 0 );

            if (hasItems)
                InventoryText.Text = "Inventario: " + "\n\nGold: " + items["GOLD"].Count + "\nSilver: " + items["SILVER"].Count +
                                     "\nIron: " + items["IRON"].Count +
                                     "\nFish: " + items["NORMALFISH"].Count + "\nYellow Fish: " + items["YELLOWFISH"].Count +
                                     "\nSpiral Coral: " + items["SPIRALCORAL"].Count + "\nNormal Coral: " + items["NORMALCORAL"].Count +
                                     "\nTree Coral: " + items["TREECORAL"].Count;
            else
                InventoryText.Text = Constants.INVENTORY_TEXT_WITHOUT_ITEMS;
        }

        private void UpdateSprite(DrawSprite sprite, float percentage, float max)
        {
            sprite.Scaling = new TGCVector2((percentage / max) * sprite.ScalingInitial.X, sprite.ScalingInitial.Y);
        }
    }
}
