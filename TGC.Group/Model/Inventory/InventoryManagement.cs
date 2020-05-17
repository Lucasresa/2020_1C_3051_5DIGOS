using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Collision;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.Draw;
using TGC.Group.Utils;

namespace TGC.Group.Model.Inventory
{
    class InventoryManagement
    {
        #region Atributos
        struct Constants
        {
            public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);
        }

        private string MediaDir, ShadersDir;
        private TgcD3dInput Input;
        private bool showInventory { get; set; }
        private TgcText2D DrawText = new TgcText2D();
        private Sprite lookAt;
        private (int posX, int posY) mouseCenter;
        private Ray ray;
        public Dictionary<string, List<CommonRigidBody>> items;
        private List<CommonRigidBody> gold = new List<CommonRigidBody>();
        private List<CommonRigidBody> iron = new List<CommonRigidBody>();
        private List<CommonRigidBody> fish = new List<CommonRigidBody>();
        private List<CommonRigidBody> normalCoral = new List<CommonRigidBody>();
        private List<CommonRigidBody> rock = new List<CommonRigidBody>();
        private List<CommonRigidBody> silver = new List<CommonRigidBody>();
        private List<CommonRigidBody> spiralCoral = new List<CommonRigidBody>();
        private List<CommonRigidBody> treeCoral = new List<CommonRigidBody>();
        private List<CommonRigidBody> yellowFish = new List<CommonRigidBody>();

        private bool hasARow = false;
        public bool hasADivingHelmet = false;
        #endregion

        #region Constructor
        public InventoryManagement(string mediaDir, string shadersDir, TgcD3dInput input)
        {
            Input = input;
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            initializer();
        }
        #endregion

        #region Metodos
        private void initializer()
        {
            ray = new Ray(Input);
            initializerDiccionary();

            lookAt = new Sprite(MediaDir, ShadersDir);
            lookAt.setInitialSprite(new TGCVector2(1, 1), "mira");
            mouseCenter.posX = (Constants.screen.width - lookAt.sprite.texture.Size.Width) / 2;
            mouseCenter.posY = (Constants.screen.height - lookAt.sprite.texture.Size.Height) / 2;
            lookAt.sprite.Position = new TGCVector2(mouseCenter.posX, mouseCenter.posY);
        }

        public void Update(TgcD3dInput input, DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody, bool lockCam)
        {
            if (lockCam)
                lookAt.sprite.Position = new TGCVector2(Cursor.Position.X, Cursor.Position.Y);
            else
                lookAt.sprite.Position = new TGCVector2(mouseCenter.posX, mouseCenter.posY);

            showInventory = lockCam;

            if (input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                findItem(dynamicsWorld, ref commonRigidBody);
        }

        public void Render()
        {
            lookAt.Render();
            if (showInventory)
                DrawText.drawText("Gold: " + gold.Count +
                                   "\nSilver: " + silver.Count +
                                   "\nRocas: " + rock.Count +
                                   "\niron: " + iron.Count +
                                   "\nFish: " + fish.Count +
                                   "\nYellow Fish: " + yellowFish.Count +
                                   "\nSpiral Coral: " + spiralCoral.Count +
                                   "\nNormal Coral: " + normalCoral.Count +
                                   "\nTree Coral: " + treeCoral.Count,
                                    250, 300, Color.White);
        }

        public void Dispose()
        {
            DisposeAll(gold);
            DisposeAll(silver);
            DisposeAll(rock);
            DisposeAll(iron);
            DisposeAll(fish);
            DisposeAll(yellowFish);
            DisposeAll(spiralCoral);
            DisposeAll(normalCoral);
            DisposeAll(treeCoral);
            lookAt.Dispose();
        }

        private void DisposeAll(List<CommonRigidBody> list)
        {
            list.ForEach(rigidBody => rigidBody.Dispose());
            list.RemoveRange(0, list.Count);
        }

        private bool isAFish(CommonRigidBody rigidBody)
        {
            var name = rigidBody.getName().ToLower();
            return name.Contains("fish");
        }

        private void splitItems(CommonRigidBody lookAtRigidBody)
        {
            var name = lookAtRigidBody.getName();
            var key = name.Substring(0, name.IndexOf("_"));
            items[key].Add(lookAtRigidBody);
        }

        private void findItem(DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody)
        {
            var item = commonRigidBody.Find(rigidBody =>
            { return isAFish(rigidBody) && !(hasARow) ? false : ray.intersectsWithObject(rigidBody.getAABB(), 500); });

            if (item != null)
            {
                splitItems(item);
                dynamicsWorld.RemoveRigidBody(item.body);
                commonRigidBody.Remove(item);
            }
        }

        private void initializerDiccionary()
        {
            items = new Dictionary<string, List<CommonRigidBody>>();
            items.Add("gold", gold);
            items.Add("silver", silver);
            items.Add("rock-n", rock);
            items.Add("iron", iron);
            items.Add("fish", fish);
            items.Add("yellowFish", yellowFish);
            items.Add("spiralCoral", spiralCoral);
            items.Add("normalCoral", normalCoral);
            items.Add("treeCoral", treeCoral);
        }
        #endregion
    }
}
