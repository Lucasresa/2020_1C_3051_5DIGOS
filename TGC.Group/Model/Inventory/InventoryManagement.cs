using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Bullet.Bodies;

namespace TGC.Group.Model.Inventory
{
    class InventoryManagement
    {
        private string MediaDir;
        private string ShadersDir;
        private TgcD3dInput Input;
        public TgcPickingRay pickingRay { get; set; }
        private bool showInventory { get; set; }
        private TgcText2D DrawText = new TgcText2D();
        public TGCVector3 characterPosition { get; set; }
        private List<CommonRigidBody> inventory = new List<CommonRigidBody>();

        public InventoryManagement(TgcD3dInput input, string mediaDir, string shadersDir)
        {
            Input = input;
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public void Update(TgcD3dInput input, DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody)
        {
            if (Input.keyPressed(Key.J))
                showInventory = !showInventory;

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
                mesh.Scale = new TGCVector3(10, 10, 10);
                var aabb = mesh.BoundingBox;

                intersected = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabb, out TGCVector3 collisionPoint);
                inSight = Math.Sqrt(TGCVector3.LengthSq(characterPosition, collisionPoint)) < 500;
                isMatch = intersected && inSight;

                return isMatch;
            });

            if (isMatch)
            {
                inventory.Add(lookAtRigidBody);
                dynamicsWorld.RemoveRigidBody(lookAtRigidBody.body);
                commonRigidBody.Remove(lookAtRigidBody);
            }
        }

        public void Render()
        {
            if (showInventory)
                DrawText.drawText("Inventario:" + inventory.Count, 500, 300, Color.White);
        }

        public void Dispose()
        {
            inventory.RemoveRange(0, inventory.Count);
        }
    }
}
