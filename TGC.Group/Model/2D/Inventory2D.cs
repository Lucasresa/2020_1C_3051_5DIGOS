using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Utils;

namespace TGC.Group.Model._2D
{
    class Inventory2D
    {
        private struct Constants
        {
            public static int SCREEN_WIDTH = D3DDevice.Instance.Device.Viewport.Width;
            public static int SCREEN_HEIGHT = D3DDevice.Instance.Device.Viewport.Height;
            public static TGCVector2 INVENTORY_TEXT_SIZE = new TGCVector2(300, 300);
            public static TGCVector2 INVENTORY_TEXT_POSITION = new TGCVector2((SCREEN_WIDTH - INVENTORY_TEXT_SIZE.X) / 2, (SCREEN_HEIGHT - INVENTORY_TEXT_SIZE.Y) / 2);
            public static string INVENTORY_TEXT_WITHOUT_ITEMS = "Inventory without items!";
            public static TGCVector2 ITEM_SCALE = new TGCVector2(1f, 1f);
            public static TGCVector2 ITEM_SIZE = new TGCVector2(100 * ITEM_SCALE.X, 100 * ITEM_SCALE.Y);
            public static TGCVector2 ITEM_INITIAL_POSITION = new TGCVector2(SCREEN_WIDTH * 0.415f, SCREEN_HEIGHT * 0.35f); // 0.36 x 0.3
        }

        private readonly string MediaDir;
        private bool HasItems;
        private readonly DrawText InventoryText;
        private List<DrawSprite> InventoryItems;

        public Inventory2D(string mediaDir)
        {
            MediaDir = mediaDir;
            InventoryText = new DrawText();
            InventoryItems = new List<DrawSprite>();
            Init();
        }

        public void Dispose()
        {
            InventoryText.Dispose();
            InventoryItems.ForEach(item => item.Dispose());
        }

        public void Init()
        {
            InventoryText.SetTextSizeAndPosition(text: "", Constants.INVENTORY_TEXT_SIZE, Constants.INVENTORY_TEXT_POSITION);
            InventoryItems.Add(InitializerItems("Normal_Coral"));
            InventoryItems.Add(InitializerItems("Spiral_Coral"));
            InventoryItems.Add(InitializerItems("Tree_Coral"));
            InventoryItems.Add(InitializerItems("Gold"));
            InventoryItems.Add(InitializerItems("Silver"));
            InventoryItems.Add(InitializerItems("Iron"));
            InventoryItems.Add(InitializerItems("Normal_Fish"));
            InventoryItems.Add(InitializerItems("Yellow_Fish"));
            CalculateItemPosition(ref InventoryItems);
        }        

        private DrawSprite InitializerItems(string sprite)
        {
            var item = new DrawSprite(MediaDir);
            item.SetImage(sprite + ".png");
            return item;
        }

        private void CalculateItemPosition(ref List<DrawSprite> inventory)
        {
            var columns = 4;
            var count = 1;
            var position = Constants.ITEM_INITIAL_POSITION;
            inventory[0].SetInitialScallingAndPosition(Constants.ITEM_SCALE, position);

            for (int index = 1; index < inventory.Count; index++)
            {
                if (count < columns)
                {
                    position.X = inventory[index - 1].Position.X + Constants.ITEM_SIZE.X + 80;
                    position.Y = inventory[index - 1].Position.Y;
                }
                else
                {
                    position.X = Constants.ITEM_INITIAL_POSITION.X;
                    position.Y = Constants.ITEM_INITIAL_POSITION.Y + Constants.ITEM_SIZE.Y + 80;
                    count = 0;
                }
                    
                count++;
                inventory[index].SetInitialScallingAndPosition(Constants.ITEM_SCALE, position);
            }

        }
        public void Render()
        {
            if (HasItems)
                InventoryItems.ForEach(item => item.Render());
            else
                InventoryText.Render();
        }

        public void UpdateItems(Dictionary<string, List<string>> items)
        {
            HasItems = items.Values.ToList().Any(listItems => listItems.Count > 0);

            if (HasItems)
                InventoryText.Text = "Inventory: " + "\n\nGold: " + items["GOLD"].Count + "\nSilver: " + items["SILVER"].Count +
                                     "\nIron: " + items["IRON"].Count +
                                     "\nFish: " + items["NORMALFISH"].Count + "\nYellow Fish: " + items["YELLOWFISH"].Count +
                                     "\nSpiral Coral: " + items["SPIRALCORAL"].Count + "\nNormal Coral: " + items["NORMALCORAL"].Count +
                                     "\nTree Coral: " + items["TREECORAL"].Count;
            else
                InventoryText.Text = Constants.INVENTORY_TEXT_WITHOUT_ITEMS;
        }
    }
}
