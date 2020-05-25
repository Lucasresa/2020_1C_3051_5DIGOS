using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Utils;

namespace TGC.Group.Model.Objects
{
    class Character
    {
        struct Constants
        {
            public static TGCVector3 outdoorPosition = new TGCVector3(1300, 3505, 20);
            public static TGCVector3 indoorPosition = new TGCVector3(515, -2340, -40);
            public static float speed = 550f;
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

        private readonly BulletRigidBodyFactory RigidBodyFactory = BulletRigidBodyFactory.Instance;
        private TGCVector3 movementDirection;
        private readonly CameraFPS Camera;
        private readonly TgcD3dInput Input;
        private float prevLatitude;
        private float Gravity { get { return Body.CenterOfMassPosition.Y < 0 ? -200 : -5; } }

        public RigidBody Body { get; set; }
        public bool DamageReceived { get; set; }
        public bool IsInsideShip { get { return Camera.Position.Y < 0; } }
        public bool IsOutsideShip { get => !IsInsideShip; }
        public bool IsOutOfWater { get { return Camera.Position.Y > 3505; } }

        public bool LooksAtTheHatch { get; set; }
        public bool CanAtack { get; set; }
        public bool NearShip { get; set; }

        public bool HasWeapon { get; set; }
        public bool HasDivingHelmet { get; set; }
        public bool CanFish { get; set; }

        public Character(CameraFPS camera, TgcD3dInput input)
        {
            Camera = camera;
            Input = input;
            Init();
        }

        public void ChangePosition(TGCVector3 newPosition)
        {
            Body.CenterOfMassTransform = TGCMatrix.Translation(newPosition).ToBulletMatrix();
            Camera.Position = new TGCVector3(Body.CenterOfMassPosition);
            RestartBodySpeed();
        }

        public void Dispose()
        {
            Body.Dispose();
        }

        private void Init()
        {
            prevLatitude = Camera.Latitude;
            Constants.planeDirector.TransformCoordinate(TGCMatrix.RotationY(FastMath.PI_HALF));
            Body = RigidBodyFactory.CreateCapsule(Constants.capsuleRadius, Constants.capsuleSize, Constants.indoorPosition, 1f, false);
            Body.CenterOfMassTransform = TGCMatrix.Translation(Constants.indoorPosition).ToBulletMatrix();
        }

        private void InsideMovement(TGCVector3 director, TGCVector3 sideDirector, float speed)
        {
            if (Input.keyDown(Key.W))
                Body.LinearVelocity = director.ToBulletVector3() * speed;

            if (Input.keyDown(Key.S))
                Body.LinearVelocity = director.ToBulletVector3() * -speed;

            if (Input.keyDown(Key.A))
                Body.LinearVelocity = sideDirector.ToBulletVector3() * -speed;

            if (Input.keyDown(Key.D))
                Body.LinearVelocity = sideDirector.ToBulletVector3() * speed;
        }

        private void OutsideMovement(TGCVector3 director, TGCVector3 sideDirector, float speed, Skybox skybox)
        {
            if (skybox.CameraIsNearBorder(Camera))
            {
                Body.ApplyCentralImpulse(movementDirection.ToBulletVector3() * -100);
                return;
            }

            if (Input.keyDown(Key.W))
            {
                Body.LinearVelocity = director.ToBulletVector3() * speed;
                movementDirection = director;
            }

            if (Input.keyDown(Key.S))
            {
                Body.LinearVelocity = director.ToBulletVector3() * -speed;
                movementDirection = -director;
            }

            if (Input.keyDown(Key.A))
            {
                Body.LinearVelocity = sideDirector.ToBulletVector3() * -speed;
                movementDirection = -sideDirector;
            }

            if (Input.keyDown(Key.D))
            {
                Body.LinearVelocity = sideDirector.ToBulletVector3() * speed;
                movementDirection = sideDirector;
            }

            if (Input.keyDown(Key.Space))
                Body.LinearVelocity = Vector3.UnitY * speed;

            if (Input.keyDown(Key.LeftControl))
                Body.LinearVelocity = Vector3.UnitY * -speed;
        }

        private void RestartBodySpeed()
        {
            Body.LinearVelocity = Vector3.Zero;
            Body.AngularVelocity = Vector3.Zero;
        }

        private void RestartSpeedForKeyUp()
        {
            if ( Input.keyUp(Key.W) || Input.keyUp(Key.S) || Input.keyUp(Key.A) || Input.keyUp(Key.D) ||
                    Input.keyUp(Key.Space) || Input.keyUp(Key.LeftControl) )
                RestartBodySpeed();
        }
        
        public void Update(Skybox skybox)
        {
            var speed = Constants.speed;
            var director = Camera.Direction;
            var sideRotation = Camera.Latitude - prevLatitude;
            var sideDirector = Constants.planeDirector;
            sideDirector.TransformCoordinate(TGCMatrix.RotationY(sideRotation));

            Body.ActivationState = ActivationState.ActiveTag;
            Body.AngularVelocity = Vector3.Zero;

            if (IsOutOfWater)
                Body.ApplyCentralImpulse(Vector3.UnitY * -3);
            else if (!IsInsideShip)
                OutsideMovement(director, sideDirector, speed, skybox);
            else
                InsideMovement(director, sideDirector, speed);

            RestartSpeedForKeyUp();

            Body.LinearVelocity += TGCVector3.Up.ToBulletVector3() * Gravity;
            Camera.Position = new TGCVector3(Body.CenterOfMassPosition) + Constants.cameraHeight;
        }

        public void Teleport()
        {
            if ( LooksAtTheHatch ) ChangePosition(Constants.outdoorPosition);
            if ( NearShip ) ChangePosition(Constants.indoorPosition);
        }
    }
}
