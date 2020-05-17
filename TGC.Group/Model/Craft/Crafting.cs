using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.Inventory;

namespace TGC.Group.Model.Craft
{
    class Crafting
    {

        private string MediaDir, ShadersDir;
        Dictionary<string, List<CommonRigidBody>> Items;

        public Crafting(string mediaDir, string shadersDir, Dictionary<string, List<CommonRigidBody>> items)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Items = items;
        }

        public void craftWeapon()
        {
            if (Items["rock-n"].Count() >= 2 && Items["silver"].Count() >= 2)
            {
                Items["rock-n"].RemoveRange(0, 2);
                Items["silver"].RemoveRange(0, 2);
                MessageBox.Show("Se crafteo un arma exitosamente.");
            }
        }

        public void craftRod()
        {
            if (Items["spiralCoral"].Count() >= 1 && Items["normalCoral"].Count() >= 1 && Items["treeCoral"].Count() >= 1 && Items["iron"].Count() >= 1)
            {
                Items["spiralCoral"].RemoveRange(0, 1);
                Items["normalCoral"].RemoveRange(0, 1);
                Items["treeCoral"].RemoveRange(0, 1);
                Items["iron"].RemoveRange(0, 1);
                MessageBox.Show("Se crafteo una caña exitosamente.");
                // hasARow = true;
            }
        }

        public void craftDivingHelmet()
        {
            if (Items["gold"].Count() >= 4)
            {
                Items["gold"].RemoveRange(0, 4);
                MessageBox.Show("Se crafteo una casco de buceo exitosamente.");
                // hasADivingHelmet = true;
            }
        }
    }
}