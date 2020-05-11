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

namespace TGC.Group.Model.Inventory
{
    class InventoryManagement
    {
        struct Constants
        {
            public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);
        }

        private string MediaDir;
        private string ShadersDir;
        private TgcD3dInput Input;
        public TgcPickingRay pickingRay { get; set; }
        private bool showInventory { get; set; }
        private TgcText2D DrawText = new TgcText2D();
        public TGCVector3 characterPosition { get; set; }
        private Sprite lookAt;
        private (int posX, int posY) mouseCenter;

        private Dictionary<string, List<CommonRigidBody>> inventory;
        private List<CommonRigidBody> gold = new List<CommonRigidBody>();
        private List<CommonRigidBody> iron = new List<CommonRigidBody>();
        private List<CommonRigidBody> fish = new List<CommonRigidBody>();
        private List<CommonRigidBody> normalCoral = new List<CommonRigidBody>();
        private List<CommonRigidBody> rock = new List<CommonRigidBody>();
        private List<CommonRigidBody> silver = new List<CommonRigidBody>();
        private List<CommonRigidBody> spiralCoral = new List<CommonRigidBody>();
        private List<CommonRigidBody> treeCoral = new List<CommonRigidBody>();
        private List<CommonRigidBody> yellowFish = new List<CommonRigidBody>();

        public InventoryManagement(TgcD3dInput input, string mediaDir, string shadersDir)
        {
            Input = input;
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            initializer();
        }

        private void initializer()
        {
            inventory = new Dictionary<string, List<CommonRigidBody>>();
            inventory.Add("gold", gold);
            inventory.Add("silver", silver);
            inventory.Add("rock-n", rock);
            inventory.Add("iron", iron);
            inventory.Add("fish", fish);
            inventory.Add("yellowFish", yellowFish);
            inventory.Add("spiralCoral", spiralCoral);
            inventory.Add("normalCoral", normalCoral);
            inventory.Add("treeCoral", treeCoral);

            lookAt = new Sprite(MediaDir, ShadersDir);
            lookAt.setInitialSprite(new TGCVector2(1, 1), "mira");
            mouseCenter.posX = (Constants.screen.width - lookAt.sprite.texture.Size.Width) / 2;
            mouseCenter.posY = (Constants.screen.height - lookAt.sprite.texture.Size.Height) / 2;
            lookAt.sprite.Position = new TGCVector2(mouseCenter.posX, mouseCenter.posY);
        }

        public void Update(TgcD3dInput input, DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody, bool lockCam)
        {
            if (lockCam)
                lookAt.sprite.Position = new TGCVector2 (Cursor.Position.X, Cursor.Position.Y);
            else
                lookAt.sprite.Position = new TGCVector2(mouseCenter.posX, mouseCenter.posY);

            showInventory = lockCam;

            if (input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                addInventory(dynamicsWorld, ref commonRigidBody);
        }

        private void addInventory(DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody)
        {
            bool isMatch = false;
            bool intersected = false;
            bool inSight = false;
            pickingRay.updateRay();

            var lookAtRigidBody = commonRigidBody.Find(rigidBody =>
            {
                var mesh = rigidBody.Mesh;
                var aabb = mesh.BoundingBox;

                intersected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out TGCVector3 collisionPoint);
                inSight = Math.Sqrt(TGCVector3.LengthSq(characterPosition, collisionPoint)) < 500;
                isMatch = intersected && inSight;

                return isMatch;
            });

            if (isMatch)
            {
                splitItems(lookAtRigidBody);
                dynamicsWorld.RemoveRigidBody(lookAtRigidBody.body);
                commonRigidBody.Remove(lookAtRigidBody);
            }
        }

        private void splitItems(CommonRigidBody lookAtRigidBody)
        {
            var name = lookAtRigidBody.Mesh.Name;
            var key = name.Substring(0, name.IndexOf("_"));
            inventory[key].Add(lookAtRigidBody);
        }

        public void Render()
        {
            lookAt.Render();
            if (showInventory)
                DrawText.drawText("Inventario:" + inventory.Count, 500, 300, Color.White);
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
    }
}
