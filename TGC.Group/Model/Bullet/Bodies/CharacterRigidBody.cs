using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Model.Draw;
using TGC.Group.Model.Inventory;
using TGC.Group.Model.Meshes;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    class CharacterRigidBody
    {
        #region Atributos
        struct Constants
        {
            public static TGCVector3 indoorPosition;
            public static TGCVector3 outdoorPosition;
            public static float speed = 450f;
            public static TGCVector3 cameraHeight = new TGCVector3(0, 85, 0);
            public static TGCVector3 planeDirector
            {
                get
                {
                    var director = new TGCVector3(-1, 0, 0);
                    director.TransformCoordinate(TGCMatrix.RotationY(FastMath.PI_HALF));
                    return director;
                }
            }
            public static float capsuleSize = 160f;
            public static float capsuleRadius = 40f;
        }

        private string MediaDir, ShadersDir;
        private TgcText2D DrawText = new TgcText2D();
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        private CameraFPS Camera;
        private TgcD3dInput input;
        private TgcPickingRay pickingRay;
        private CharacterStatus status;
        private InventoryManagement inventory;
        private float prevLatitude;
        private bool showEnterShipInfo;
        public TgcBoundingAxisAlignBox aabbShip;
        public RigidBody body;
        private Weapon weapon;

        public CharacterStatus Status { get { return status; } }
        #endregion

        #region Constructor
        public CharacterRigidBody(TgcD3dInput Input, CameraFPS camera, string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Camera = camera;
            Constants.indoorPosition = camera.getIndoorPosition();
            Constants.outdoorPosition = camera.getOutdoorPosition();
            input = Input;
            Init();
        }
        #endregion

        #region Metodos
        private void Init()
        {
            weapon = new Weapon(MediaDir, ShadersDir);
            inventory = new InventoryManagement(MediaDir, ShadersDir, input);
            status = new CharacterStatus(MediaDir, ShadersDir, input, inventory);
            createPickingRay();

            prevLatitude = Camera.latitude;
            Constants.planeDirector.TransformCoordinate(TGCMatrix.RotationY(FastMath.PI_HALF));

            #region Create rigidBody
            body = rigidBodyFactory.CreateCapsule(Constants.capsuleRadius, Constants.capsuleSize, Constants.indoorPosition, 1f, false);
            body.CenterOfMassTransform = TGCMatrix.Translation(Constants.indoorPosition).ToBulletMatrix();
            #endregion
        }

        public void Update(DiscreteDynamicsWorld dynamicsWorld, ref List<CommonRigidBody> commonRigidBody, 
                            float elapsedTime, SharkRigidBody shark)
        {
            var speed = Constants.speed;
            
            inventory.Update(input, dynamicsWorld, ref commonRigidBody, Camera.lockCam);

            if (Camera.lockCam)
                return;

            canRecoverOxygen();
            status.Update();
            updateInventoryWithCharacterPosition();
            teleport();

            #region Movimiento 
            body.ActivationState = ActivationState.ActiveTag;
            body.AngularVelocity = TGCVector3.Empty.ToBulletVector3();

            var director = Camera.LookAt - Camera.position;
            director.Normalize();

            var sideRotation = Camera.latitude - prevLatitude;
            var sideDirector = Constants.planeDirector;
            sideDirector.TransformCoordinate(TGCMatrix.RotationY(sideRotation));

            if (!isOutOfWater())
            {
                if (!isInsideShip())
                    outsideMovement(director, sideDirector, speed);
                else
                    insideMovement(director, sideDirector, speed);
            }
            else
                body.ApplyCentralImpulse(Vector3.UnitY * -5);

            if (input.keyUp(Key.W) || input.keyUp(Key.S) || input.keyUp(Key.A) || input.keyUp(Key.D) ||
                input.keyUp(Key.LeftControl) || input.keyUp(Key.Space))
            {
                body.LinearVelocity = Vector3.Zero;
                body.AngularVelocity = Vector3.Zero;
            }

            if (input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_RIGHT) && !weapon.AtackLocked)
            {
                weapon.ActivateAtackMove();
                if (CheckIfCanAtack(shark))
                    shark.ReceiveDamage(50);
            }

            body.LinearVelocity += TGCVector3.Up.ToBulletVector3() * getGravity();
            Camera.position = new TGCVector3(body.CenterOfMassPosition) + Constants.cameraHeight;
            weapon.Update(Camera, director, elapsedTime);
            #endregion
        }

        public void Render()
        {
            status.Render();
            inventory.Render();
            weapon.Render();
            if(showEnterShipInfo)
                DrawText.drawText("PRESIONA E PARA ENTRAR A LA NAVE", 500, 400, Color.White);
        }

        public void Dispose()
        {
            body.Dispose();
            status.Dispose();
            inventory.Dispose();
            weapon.Dispose();
        }

        public void teleport()
        {
            if (isInsideShip() || isNearShip())
                showEnterShipInfo = true;
            else
                showEnterShipInfo = false;

            if (input.keyPressed(Key.E))
            {
                if(isInsideShip())
                    changePosition(Constants.outdoorPosition);
                if (isNearShip())
                    changePosition(Constants.indoorPosition);
            }
        }

        public bool isInsideShip()
        {
            return Camera.position.Y < 0;
        }

        private bool CheckIfCanAtack(SharkRigidBody shark)
        {
            pickingRay.updateRay();

            var intersect = TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, shark.Mesh.BoundingBox, out TGCVector3 collisionPoint);
            var inSight = Math.Sqrt(TGCVector3.LengthSq(Camera.position, collisionPoint)) < 100 || isInsideShip();
            return intersect && inSight;
        }

        private void changePosition(TGCVector3 newPosition)
        {
            body.CenterOfMassTransform = TGCMatrix.Translation(newPosition).ToBulletMatrix();
            Camera.position = new TGCVector3(body.CenterOfMassPosition);
            body.LinearVelocity = Vector3.Zero;
            body.AngularVelocity = Vector3.Zero;
        }

        private float getGravity()
        {
            return body.CenterOfMassPosition.Y < 0 ? -200 : -5;
        }

        private bool isOutOfWater()
        {
            return Camera.position.Y > 3505;
        }


        private void createPickingRay()
        {
            pickingRay = new TgcPickingRay(input);
            inventory.pickingRay = pickingRay;
        }

        private void canRecoverOxygen()
        {
            status.canBreathe = isOutOfWater() || isInsideShip();
        }

        private void updateInventoryWithCharacterPosition()
        {
            inventory.characterPosition = Camera.position;
        }

        private bool isNearShip()
        {
            pickingRay.updateRay();

            var intersected = intersectBetweenCharacterAndShip(out TGCVector3 collisionPoint);
            var inSight = distanceBetweenCharacterAndShip(collisionPoint);
           
            return intersected && inSight;
        }

        private bool intersectBetweenCharacterAndShip( out TGCVector3 collisionPoint )
        {   // TODO: Despues podriamos considerar la idea de si esta mirando el techo
            collisionPoint = TGCVector3.Empty;
            if (isInsideShip())
                return true;
            else
                return TgcCollisionUtils.intersectRayAABB(pickingRay.Ray, aabbShip, out collisionPoint);
        }

        private bool distanceBetweenCharacterAndShip(TGCVector3 collisionPoint)
        {
            return Math.Sqrt(TGCVector3.LengthSq(Camera.position, collisionPoint)) < 500 || isInsideShip();
        }

        #region Movimientos
        private void insideMovement(TGCVector3 director, TGCVector3 sideDirector, float speed)
        {
            if (input.keyDown(Key.W))
                body.LinearVelocity = director.ToBulletVector3() * speed;

            if (input.keyDown(Key.S))
                body.LinearVelocity = director.ToBulletVector3() * -speed;

            if (input.keyDown(Key.A))
                body.LinearVelocity = sideDirector.ToBulletVector3() * -speed;

            if (input.keyDown(Key.D))
                body.LinearVelocity = sideDirector.ToBulletVector3() * speed;
        }

        private void outsideMovement(TGCVector3 director, TGCVector3 sideDirector, float speed)
        {
            if (input.keyDown(Key.W))
                body.LinearVelocity = director.ToBulletVector3() * speed;

            if (input.keyDown(Key.S))
                body.LinearVelocity = director.ToBulletVector3() * -speed;

            if (input.keyDown(Key.A))
                body.LinearVelocity = sideDirector.ToBulletVector3() * -speed;

            if (input.keyDown(Key.D))
                body.LinearVelocity = sideDirector.ToBulletVector3() * speed;

            if (input.keyDown(Key.Space))
                body.LinearVelocity = Vector3.UnitY * speed;

            if (input.keyDown(Key.LeftControl))
                body.LinearVelocity = Vector3.UnitY * -speed;
        }
        #endregion

        #endregion
    }
}
