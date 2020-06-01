using System.Collections.Generic;
using System.Linq;
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
        }

        private readonly DrawText InventoryText;

        public Inventory2D()
        {
            InventoryText = new DrawText();
            Init();
        }
        
        public void Dispose() => InventoryText.Dispose();

        public void Init() =>
            InventoryText.SetTextSizeAndPosition(text: "", Constants.INVENTORY_TEXT_SIZE, Constants.INVENTORY_TEXT_POSITION);

        public void Render() => InventoryText.Render();

        public void UpdateItems(Dictionary<string, List<string>> items)
        {
            var hasItems = items.Values.ToList().Any(listItems => listItems.Count > 0);

            if (hasItems)
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
