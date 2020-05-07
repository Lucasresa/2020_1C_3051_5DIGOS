using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
using TGC.Core.BoundingVolumes;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    class CharacterRigidBody : RigidBody
    {
        struct Constants
        {
            public static float speed = 450f;
            public static TGCVector3 cameraHeight = new TGCVector3(0, 85, 0);
            public static float capsuleSize = 160f;
            public static float capsuleRadius = 40f;

        }

        private CameraFPS Camera;
        private TGCVector3 position;
        private TGCVector3 indoorPosition;
        private TGCVector3 outdoorPosition;

        public CharacterRigidBody(CameraFPS camera)
        {
            Camera = camera;
            indoorPosition = camera.getIndoorPosition();
            outdoorPosition = camera.getOutdoorPosition();
        }

        public override void Init()
        {
            if (Camera.isOutside) position = Camera.getOutdoorPosition();
            else position = Camera.getIndoorPosition();

            #region Create rigidBody
            body = rigidBodyFactory.CreateCapsule(Constants.capsuleRadius, Constants.capsuleSize, position, 1f, false);
            body.CenterOfMassTransform = TGCMatrix.Translation(position).ToBulletMatrix();
            #endregion
        }

        public override void Update(TgcD3dInput input, float elapsedTime)
        {
            var speed = Constants.speed;
            body.ActivationState = ActivationState.ActiveTag;

            #region Movimiento 

            body.AngularVelocity = TGCVector3.Empty.ToBulletVector3();

            var director = Camera.LookAt - Camera.position;
            director.Normalize();

            if (!isOutOfWater())
            {
                if (input.keyDown(Key.W))
                {
                    body.LinearVelocity = director.ToBulletVector3() * speed;
                }

                if (input.keyDown(Key.S))
                {
                    body.LinearVelocity = director.ToBulletVector3() * -speed;
                }

                if (input.keyDown(Key.A))
                {
                    director.TransformCoordinate(TGCMatrix.RotationY(FastMath.PI_HALF));
                    body.LinearVelocity = director.ToBulletVector3() * -speed;
                }

                if (input.keyDown(Key.D))
                {
                    director.TransformCoordinate(TGCMatrix.RotationY(FastMath.PI_HALF));
                    body.LinearVelocity = director.ToBulletVector3() * speed;
                }

                if (input.keyDown(Key.Space))
                {
                    body.LinearVelocity = Vector3.UnitY * speed;
                }

                if (input.keyDown(Key.LeftControl))
                {
                    body.LinearVelocity = Vector3.UnitY * -speed;
                }
            }
            else
                body.ApplyCentralImpulse(Vector3.UnitY * -5);

            if (input.keyUp(Key.W) || input.keyUp(Key.S) || input.keyUp(Key.A) || input.keyUp(Key.D) 
                 || input.keyUp(Key.LeftControl) || input.keyUp(Key.Space))
            {
                body.LinearVelocity = Vector3.Zero;
                body.AngularVelocity = Vector3.Zero;
            }

            if (input.keyPressed(Key.E)) Teleport();

            body.LinearVelocity += TGCVector3.Up.ToBulletVector3() * getGravity();

            Camera.position = new TGCVector3(body.CenterOfMassPosition) + Constants.cameraHeight;

            #endregion
        }

        private float getGravity()
        {
            return body.CenterOfMassPosition.Y < 0 ? -200 : -5;
        }

        private bool isOutOfWater()
        {
            return Camera.position.Y > 3505;
        }

        public override void Dispose()
        {
            body.Dispose();
        }

        public override void Teleport()
        {
            if (Camera.isOutside)
                body.CenterOfMassTransform = TGCMatrix.Translation(indoorPosition).ToBulletMatrix();
            else
                body.CenterOfMassTransform = TGCMatrix.Translation(outdoorPosition).ToBulletMatrix();

            Camera.position = new TGCVector3(body.CenterOfMassPosition);
            body.LinearVelocity = Vector3.Zero;
            body.AngularVelocity = Vector3.Zero;
        }
    }
}
