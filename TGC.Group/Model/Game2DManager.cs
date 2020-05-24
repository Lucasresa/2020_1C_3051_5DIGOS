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
        }


        private List<string> Items = new List<string>();
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

        public bool ActiveInventory { get; set; }

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
            Init();
        }

        public void Dispose()
        {
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
            InventoryText.SetTextSizeAndPosition("Inventory without items!", Constants.INVENTORY_TEXT_SIZE, Constants.INVENTORY_TEXT_POSITION);
            InventoryText.Color = Color.Black;
        }

        public void Render()
        {
            LifeShark.Render();
            LifeCharacter.Render();
            OxygenCharacter.Render();
            LifeSharkText.Render();
            LifeCharacterText.Render();
            OxygenCharacterText.Render();

            if (!ActiveInventory)
                Pointer.Render();
            else
            {
                MousePointer.Position = new TGCVector2(Cursor.Position.X, Cursor.Position.Y);
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

        public void UpdateItems(List<Items> items)
        {
            if (items.Count == Items.Count)
                return;
            
            List<string> itemsName = new List<string>();
            items.ForEach(item => itemsName.Add(item.name.ToUpper()));
            Items.RemoveRange(0, Items.Count);
            Items.AddRange(itemsName);
            InventoryText.Text = "Inventario: " + "\n\nGold: " + Items.Count(item => item.Contains("GOLD")) + "\nSilver: " + Items.Count(item => item.Contains("SILVER")) +
                                 "\nRock: " + Items.Count(item => item.Contains("ROCK")) + "\nIron: " + Items.Count(item => item.Contains("IRON")) +
                                 "\nFish: " + Items.Count(item => item.Contains("NORMALFISH")) + "\nYellow Fish: " + Items.Count(item => item.Contains("YELLOW")) +
                                 "\nSpiral Coral: " + Items.Count(item => item.Contains("SPIRALCORAL")) +
                                 "\nNormal Coral: " + Items.Count(item => item.Contains("NORMALCORAL")) + "\nTree Coral: " + Items.Count(item => item.Contains("TREECORAL"));
        }

        private void UpdateSprite(DrawSprite sprite, float percentage, float max)
        {
            sprite.Scaling = new TGCVector2((percentage / max) * sprite.ScalingInitial.X, sprite.ScalingInitial.Y);
        }
    }
}
