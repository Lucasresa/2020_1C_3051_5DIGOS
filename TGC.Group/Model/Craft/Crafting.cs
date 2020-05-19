using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Input;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.Inventory;

namespace TGC.Group.Model.Craft
{
    class Crafting
    {

        private string MediaDir, ShadersDir;
        private InventoryManagement Inventory;

        public Crafting(string mediaDir, string shadersDir, InventoryManagement inventory)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Inventory = inventory;
        }

        public void craftWeapon()
        {
            if (Inventory.items["rock-n"].Count() >= 2 && Inventory.items["silver"].Count() >= 2)
            {
                Inventory.items["rock-n"].RemoveRange(0, 2);
                Inventory.items["silver"].RemoveRange(0, 2);
                MessageBox.Show("Se crafteo un arma exitosamente.");
                Inventory.hasAWeapon = true;
            }
        }

        public void craftRod()
        {
            if (Inventory.items["spiralCoral"].Count() >= 1 && Inventory.items["normalCoral"].Count() >= 1 && Inventory.items["treeCoral"].Count() >= 1 && Inventory.items["iron"].Count() >= 1)
            {
                Inventory.items["spiralCoral"].RemoveRange(0, 1);
                Inventory.items["normalCoral"].RemoveRange(0, 1);
                Inventory.items["treeCoral"].RemoveRange(0, 1);
                Inventory.items["iron"].RemoveRange(0, 1);
                MessageBox.Show("Se crafteo una caña exitosamente.");
                Inventory.hasARow = true;
            }
        }

        public void craftDivingHelmet()
        {
            if (Inventory.items["gold"].Count() >= 4)
            {
                Inventory.items["gold"].RemoveRange(0, 4);
                MessageBox.Show("Se crafteo una casco de buceo exitosamente.");
                Inventory.hasADivingHelmet = true;
            }
        }

        internal void Update(TgcD3dInput input)
        {
            if (input.keyDown(Key.M))
                craftWeapon();

            if (input.keyDown(Key.N))
                craftRod();

            if (input.keyDown(Key.B))
                craftDivingHelmet();
        }
    }
}