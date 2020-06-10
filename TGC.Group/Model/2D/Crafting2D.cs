﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Utils;

namespace TGC.Group.Model._2D
{
    class Crafting2D
    {
        private struct Constants
        {
            public static int SCREEN_WIDTH = D3DDevice.Instance.Device.Viewport.Width;
            public static int SCREEN_HEIGHT = D3DDevice.Instance.Device.Viewport.Height;
            public static TGCVector2 TEXT_SIZE = new TGCVector2(300, 300);
            public static TGCVector2 TEXT_POSITION = new TGCVector2((SCREEN_WIDTH - TEXT_SIZE.X) / 2, (SCREEN_HEIGHT - TEXT_SIZE.Y) / 2);
        }

        private readonly string MediaDir;
        private readonly DrawText InventoryText;
        private readonly DrawText CraftingText;
        private readonly List<(DrawSprite sprite, DrawText text)> InventoryItems;
        public List<(DrawSprite sprite, DrawButton button)> CraftingItems;

        private TGCVector2 Size;
        public TgcD3dInput Input { get; set; }

        public Crafting2D(string mediaDir, TgcD3dInput input)
        {
            Input = input;
            MediaDir = mediaDir;
            InventoryText = new DrawText();
            CraftingText = new DrawText();
            InventoryItems = new List<(DrawSprite, DrawText)>();
            CraftingItems = new List<(DrawSprite, DrawButton)>();
            Init();
        }

        public void Dispose()
        {
            InventoryText.Dispose();
            InventoryItems.ForEach(item => { item.sprite.Dispose(); item.text.Dispose(); });
            CraftingText.Dispose();
            CraftingItems.ForEach(item => { item.sprite.Dispose(); item.button.Dispose(); });
        }

        public void Init()
        {
            InventoryText.SetTextSizeAndPosition(text: "Inventory:", Constants.TEXT_SIZE, Constants.TEXT_POSITION);
            InventoryItems.Add(InitializerItems("NORMALCORAL"));
            InventoryItems.Add(InitializerItems("SPIRALCORAL"));
            InventoryItems.Add(InitializerItems("TREECORAL"));
            InventoryItems.Add(InitializerItems("GOLD"));
            InventoryItems.Add(InitializerItems("SILVER"));
            InventoryItems.Add(InitializerItems("IRON"));
            InventoryItems.Add(InitializerItems("NORMALFISH"));
            InventoryItems.Add(InitializerItems("YELLOWFISH"));
            CalculateItemPosition();
            CraftingText.Text = "Crafting:";
            CraftingItems.Add(InitializerCraftItem("NORMALCORAL"));
            CraftingItems.Add(InitializerCraftItem("NORMALCORAL"));
            CraftingItems.Add(InitializerCraftItem("NORMALCORAL"));
            CalculateCraftItemPosition();
        }

        private (DrawSprite, DrawText) InitializerItems(string sprite)
        {
            var item = new DrawSprite(MediaDir);
            item.SetImage(sprite + ".png");
            var text = new DrawText();
            return (item, text);
        }

        private (DrawSprite, DrawButton) InitializerCraftItem(string sprite)
        {
            var item = new DrawSprite(MediaDir);
            item.SetImage(sprite + ".png");
            var button = new DrawButton(MediaDir, Input);
            button.InitializerButton(text: "Craft", scale: new TGCVector2(0.4f, 0.4f),
                position: Constants.TEXT_POSITION);
            return (item, button);
        }

        private void CalculateItemPosition()
        {
            TGCVector2 scale;
            if (Constants.SCREEN_WIDTH < 1366)
                scale = new TGCVector2(0.732f / 2, 0.783f / 2);
            else if (FastUtils.IsNumberBetweenInterval(Constants.SCREEN_WIDTH, (1366, 1700)))
                scale = new TGCVector2(0.9f / 2, 0.9f / 2);
            else
                scale = new TGCVector2(1.2f / 2, 1.2f / 2);

            Size = new TGCVector2(100 * scale.X, 100 * scale.Y);
            TGCVector2 initialPosition = new TGCVector2(Constants.SCREEN_WIDTH * 0.39f, Constants.SCREEN_HEIGHT * 0.35f);

            var columns = 8;
            var count = 1;
            var position = initialPosition;
            InventoryItems[0].sprite.SetInitialScallingAndPosition(scale, position);
            InventoryItems[0].text.Position = new TGCVector2(InventoryItems[0].sprite.Position.X + Size.X, InventoryItems[0].sprite.Position.Y + Size.Y + 10);

            for (int index = 1; index < InventoryItems.Count; index++)
            {
                if (count < columns)
                {
                    position.X = InventoryItems[index - 1].sprite.Position.X + Size.X + 25;
                    position.Y = InventoryItems[index - 1].sprite.Position.Y;
                }
                else
                {
                    position.X = initialPosition.X;
                    position.Y = initialPosition.Y + Size.Y + 25;
                    count = 0;
                }

                count++;
                InventoryItems[index].sprite.SetInitialScallingAndPosition(scale, position);
                InventoryItems[index].text.Position = new TGCVector2(InventoryItems[index].sprite.Position.X + Size.X, InventoryItems[index].sprite.Position.Y + Size.Y + 10);
            }

            InventoryText.Position = new TGCVector2(InventoryItems[0].sprite.Position.X, InventoryItems[0].sprite.Position.Y - 60);
            CraftingText.Position = new TGCVector2(InventoryItems[0].sprite.Position.X, InventoryItems[0].text.Position.Y + 40);
        }

        private void CalculateCraftItemPosition()
        {
            TGCVector2 scale;
            if (Constants.SCREEN_WIDTH < 1366)
                scale = new TGCVector2(0.732f / 2, 0.783f / 2);
            else if (FastUtils.IsNumberBetweenInterval(Constants.SCREEN_WIDTH, (1366, 1700)))
                scale = new TGCVector2(0.9f / 2, 0.9f / 2);
            else
                scale = new TGCVector2(1.2f / 2, 1.2f / 2);

            var Size = new TGCVector2(100 * scale.X, 100 * scale.Y);
            TGCVector2 position = new TGCVector2(InventoryItems[1].sprite.Position.X, CraftingText.Position.Y + 30);                       
            
            CraftingItems[0].sprite.SetInitialScallingAndPosition(scale, position);
            CraftingItems[0].button.ChangePosition(new TGCVector2(InventoryItems[5].sprite.Position.X, position.Y + (Size.Y - CraftingItems[0].button.SizeText.Y) / 2));

            for (int index = 1; index < CraftingItems.Count; index++)
            {
                position.X = CraftingItems[index - 1].sprite.Position.X;
                position.Y = CraftingItems[index - 1].sprite.Position.Y + Size.Y + 30;

                CraftingItems[index].sprite.SetInitialScallingAndPosition(scale, position);
                CraftingItems[index].button.ChangePosition(new TGCVector2(InventoryItems[5].sprite.Position.X, position.Y + (Size.Y - CraftingItems[0].button.SizeText.Y) / 2));
            }
        }

        public void UpdateItemsCrafting() =>
            CraftingItems.ForEach(item => item.button.Update());

        public void Render()
        {
            InventoryText.Render();
            CraftingText.Render();
            InventoryItems.ForEach(item => { item.sprite.Render(); item.text.Render(); });
            CraftingItems.ForEach(item => { item.sprite.Render(); item.button.Render(); });
        }

        public void UpdateItems(Dictionary<string, List<string>> items) =>
            InventoryItems.ForEach(item => item.text.Text = "x" + items[item.sprite.Name].Count);
    }
}