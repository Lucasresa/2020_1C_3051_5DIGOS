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
        /*
        public struct Perimeter
        {
            public float xMin;
            public float xMax;
            public float zMin;
            public float zMax;
        }

        Perimeter square = new Perimeter();

        private readonly float COLUMNS = 3;
        private readonly float ROWS = 3;
        private readonly int SideX = 100;
        private readonly int SideY = 100;

        Dictionary<(int, int), Perimeter> Areas = new Dictionary<(int, int), Perimeter>();

        public void SplitToArea()
        {
            for (int row = 1; row <= ROWS; row++)
            {
                square.xMin = (((row - 1) * SideX + 20) / ROWS) + 20;
                square.xMax = (row * SideX + 20)/ ROWS + 20;

                for (int column = 1; column <= COLUMNS; column++)
                {
                    square.zMin = (((column - 1) * SideY + 20) / COLUMNS) + 20;
                    square.zMax = (column * SideY + 20) / COLUMNS + 20;
                    Areas.Add((row, column), square);
                }
            }
        }

        public Perimeter GetArea(float posX, float posZ)
        {
            return Areas.FirstOrDefault(pair => posX > pair.Value.xMin && posX < pair.Value.xMax &&
                                                posZ > pair.Value.zMin && posZ < pair.Value.zMax)
                                        .Value;
        }*/
    }
}
