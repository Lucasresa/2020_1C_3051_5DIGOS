using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
using TGC.Core.BulletPhysics;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Draw;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    class SharkRigidBody
    {
        #region Atributos
        private struct Constants
        {
            public static float MAX_LIFE = 250;
            public static TGCVector2 SHARK_HEIGHT = new TGCVector2(700, 1800);
            public static TGCVector3 scale = new TGCVector3(5, 5, 5);
            public static TGCVector3 startPosition = TGCVector3.Empty;
            public static float MaxYRotation = FastMath.PI - (FastMath.QUARTER_PI / 2);
            public static float MaxAxisRotation = FastMath.QUARTER_PI;
            public static TGCVector3 sharkBodySize = new TGCVector3(176, 154, 560);
            public static float sharkMass = 1000;
        }

        private const float SHARK_STALKER_TIME = 5;
        private const float SHARK_EVENT_TIME = 10;
        private TGCVector3 director = new TGCVector3(0, 0, 1);
        private readonly Sky skybox;
        private readonly Terrain terrain;
        private CameraFPS camera;
        private bool normalMove = false;
        private bool stalkerModeMove = false;
        private float acumulatedYRotation = 0;
        private float acumulatedXRotation = 0;
        private float seekTimeCounter = SHARK_STALKER_TIME;
        private float eventTimeCounter = SHARK_EVENT_TIME;
        private TGCMatrix TotalRotation = TGCMatrix.Identity;
        private float Life;
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        private GameEventsManager Events;

        public RigidBody body;
        public TgcMesh Mesh;

        #endregion

        #region Constructor

        public SharkRigidBody(Shark shark, Sky sky, Terrain terrain, CameraFPS camera)
        {
            Mesh = shark.Mesh;
            skybox = sky;
            this.terrain = terrain;
            this.camera = camera;
            Life = Constants.MAX_LIFE;
            Init();
        }

        #endregion

        #region Metodos

        private void Init()
        {
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * TGCMatrix.Translation(Constants.startPosition);
            body = rigidBodyFactory.CreateBox(Constants.sharkBodySize, Constants.sharkMass, Constants.startPosition, 0, 0, 0, 0, false);
        }

        public void ActivateShark(GameEventsManager events)
        {
            Events = events;
            stalkerModeMove = true;
            normalMove = true;
            var position = CalculateInitialPosition();
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * TGCMatrix.Translation(position);
            body.WorldTransform = Mesh.Transform.ToBulletMatrix();
            director = new TGCVector3(0, 0, 1);
            TotalRotation = TGCMatrix.Identity;
            seekTimeCounter = SHARK_STALKER_TIME;
            acumulatedXRotation = 0;
            acumulatedYRotation = 0;
        }

        private TGCVector3 CalculateInitialPosition()
        {
            var outOfSkyboxPosition = camera.position - new TGCVector3(0, 0, skybox.Radius + 200);
            terrain.world.interpoledHeight(outOfSkyboxPosition.X, outOfSkyboxPosition.Z, out float Y);
            return new TGCVector3(outOfSkyboxPosition.X, Y + 600, outOfSkyboxPosition.Z);
        }

        public void Update(TgcD3dInput input, float elapsedTime, CharacterStatus playerStatus)
        {
            var speed = 1000f;
            var headPosition = GetHeadPosition();

            body.ActivationState = ActivationState.ActiveTag;
            body.AngularVelocity = Vector3.Zero;

            if (!stalkerModeMove && !normalMove)
                return;

            if (stalkerModeMove && CanSeekPlayer(out float rotationAngle, out TGCVector3 rotationAxis))
                PerformStalkerMove(elapsedTime, speed, rotationAngle, rotationAxis);
            else if (normalMove)
                PerformNormalMove(elapsedTime, speed, headPosition);

            if (IsCollapsinWithPlayer())
            {
                playerStatus.ReceiveDamage(30);
                ChangeSharkWay();
            }

            if (seekTimeCounter <= 0 || input.keyDown(Key.P))
                ChangeSharkWay();

            if(eventTimeCounter <= 0)
                ManageEndOfAttack(elapsedTime);

            //if (input.keyPressed(Key.T)) normalMove = !normalMove;
            //if (input.keyPressed(Key.Y)) stalkerModeMove = !stalkerModeMove;
        }

        public void Render()
        {
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * new TGCMatrix(body.InterpolationWorldTransform);
            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.BoundingBox.Render();
            Mesh.Render();
        }

        public void Dispose()
        {
            body.Dispose();
            Mesh.Dispose();
        }

        private void ManageEndOfAttack(float elapsedTime)
        {
            if (stalkerModeMove)
                ChangeSharkWay();
            stalkerModeMove = false;
            if (!skybox.Contains(body))
            {
                EndSharkAttack();
                Events.InformFinishFromAttack();
            }
        }

        private void EndSharkAttack()
        {
            normalMove = false;
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * TGCMatrix.Translation(Constants.startPosition);
            body.WorldTransform = Mesh.Transform.ToBulletMatrix();
            director = new TGCVector3(0, 0, 1);
            TotalRotation = TGCMatrix.Identity;
            seekTimeCounter = 0;
            eventTimeCounter = SHARK_EVENT_TIME;
            acumulatedXRotation = 0;
            acumulatedYRotation = 0;
        }

        #region Movements
        private void PerformNormalMove(float elapsedTime, float speed, TGCVector3 headPosition)
        {
            var XRotation = 0f;
            var YRotation = 0f;
            terrain.world.interpoledHeight(headPosition.X, headPosition.Z, out float floorHeight);
            var distanceToFloor = body.CenterOfMassPosition.Y - floorHeight;
            var XRotationStep = FastMath.PI * 0.1f * elapsedTime;
            var YRotationStep = FastMath.PI * 0.4f * elapsedTime;

            if (distanceToFloor < Constants.SHARK_HEIGHT.X - 150 && acumulatedXRotation < Constants.MaxAxisRotation)
                XRotation = XRotationStep;
            else if (IsNumberBetweenInterval(distanceToFloor, Constants.SHARK_HEIGHT) && acumulatedXRotation > 0.0012)
                XRotation = -XRotationStep;
            if (distanceToFloor > Constants.SHARK_HEIGHT.Y + 150 && acumulatedXRotation > -Constants.MaxAxisRotation)
                XRotation = -XRotationStep;
            else if (IsNumberBetweenInterval(distanceToFloor, Constants.SHARK_HEIGHT) && acumulatedXRotation < -0.0012)
                XRotation = XRotationStep;

            if (!skybox.Contains(body) && FastMath.Abs(acumulatedYRotation) < Constants.MaxYRotation)
                YRotation = YRotationStep * RotationYSign();
            else
                acumulatedYRotation = 0;

            acumulatedXRotation += XRotation;
            acumulatedYRotation += YRotation;

            body.ActivationState = ActivationState.ActiveTag;
            TGCMatrix rotation = TGCMatrix.Identity;
            if (XRotation != 0 || FastMath.Abs(acumulatedXRotation) > 0.0012)
            {
                var rotationAxis = TGCVector3.Cross(TGCVector3.Up, director);
                director.TransformCoordinate(TGCMatrix.RotationAxis(rotationAxis, XRotation));
                rotation = TGCMatrix.RotationAxis(rotationAxis, XRotation);
                speed /= 1.5f;
            }
            else if (YRotation != 0)
            {
                director.TransformCoordinate(TGCMatrix.RotationY(YRotation));
                rotation = TGCMatrix.RotationY(YRotation);
            }
            TotalRotation *= rotation;
            Mesh.Transform = TotalRotation * TGCMatrix.Translation(new TGCVector3(body.CenterOfMassPosition));
            body.WorldTransform = Mesh.Transform.ToBulletMatrix();
            body.LinearVelocity = director.ToBulletVector3() * -speed;
        }

        private void PerformStalkerMove(float elapsedTime, float speed, float rotationAngle, TGCVector3 rotationAxis)
        {
            var actualDirector = -1 * director;
            seekTimeCounter -= elapsedTime;
            eventTimeCounter -= elapsedTime;
            var RotationStep = FastMath.PI * 0.3f * elapsedTime;

            if (rotationAngle <= RotationStep)
                return;
            actualDirector.TransformCoordinate(TGCMatrix.RotationAxis(rotationAxis, RotationStep));
            var newRotation = TGCMatrix.RotationAxis(rotationAxis, RotationStep);
            TotalRotation *= newRotation;

            Mesh.Transform = TotalRotation * TGCMatrix.Translation(new TGCVector3(body.CenterOfMassPosition));
            body.WorldTransform = Mesh.Transform.ToBulletMatrix();

            director = -1 * actualDirector;
            body.LinearVelocity = director.ToBulletVector3() * -speed;
        }
        #endregion

        public void ReceiveDamage(float damage)
        {
            Life = FastMath.Clamp(Life - damage, 0, Constants.MAX_LIFE);
        }

        #endregion

        #region Private Methods
        private TGCVector3 ObtainNormalVector(TGCVector3 vectorA, TGCVector3 vectorB)
        {
            return TGCVector3.Normalize(TGCVector3.Cross(vectorA, vectorB));
        }

        private bool IsCollapsinWithPlayer()
        {
            var distanceToPlayer = (camera.position - GetHeadPosition()).Length(); 
            return distanceToPlayer < 100;
        }

        private void ChangeSharkWay()
        {
            var rotation = TGCMatrix.RotationY(FastMath.PI_HALF * -RotationYSign());
            director = new TGCVector3(0, 0, 1);
            director.TransformCoordinate(rotation);
            Mesh.Transform = rotation * TGCMatrix.Translation(new TGCVector3(body.CenterOfMassPosition));
            body.WorldTransform = Mesh.Transform.ToBulletMatrix();
            seekTimeCounter = SHARK_STALKER_TIME;
            acumulatedXRotation = 0;
            acumulatedYRotation = 0;
            TotalRotation = rotation;
        }

        private bool CanSeekPlayer(out float rotationAngle, out TGCVector3 rotationAxis)
        {
            var actualDirector = -1 * director;
            var directorToPlayer = TGCVector3.Normalize(camera.position - new TGCVector3(body.CenterOfMassPosition));
            var NormalVectorFromDirAndPlayer = ObtainNormalVector(actualDirector, directorToPlayer);
            if (NormalVectorFromDirAndPlayer.Length() > 0.98f)
            {
                var dotProduct = TGCVector3.Dot(actualDirector, directorToPlayer) / (director.Length() * directorToPlayer.Length());
                if (dotProduct < 1)
                {
                    var RotationToPlayer = FastMath.Acos(dotProduct);
                    if (RotationToPlayer <= FastMath.QUARTER_PI)
                    {
                        rotationAngle = RotationToPlayer;
                        rotationAxis = NormalVectorFromDirAndPlayer;
                        return true;
                    }                    
                }
            }
            rotationAngle = 0;
            rotationAxis = TGCVector3.Empty;
            return false;
        }

        private bool IsNumberBetweenInterval(float number, TGCVector2 interval)
        {
            return number > interval.X && number < interval.Y;
        }

        private float RotationYSign()
        {
            var bodyToSkyboxCenterVector = TGCVector3.Normalize(skybox.getSkyboxCenter() - new TGCVector3(body.CenterOfMassPosition));
            var actualDirector = -1 * director;
            var normalVector = TGCVector3.Cross(actualDirector, bodyToSkyboxCenterVector);
            return normalVector.Y > 0 ? 1 : -1;
        }

        private TGCVector3 GetHeadPosition()
        {
            return new TGCVector3(body.CenterOfMassPosition) + director * -560;
        }
        #endregion
    }
}
