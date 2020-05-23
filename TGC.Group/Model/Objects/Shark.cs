using BulletSharp;
using BulletSharp.Math;
using TGC.Core.BulletPhysics;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;

namespace TGC.Group.Model.Objects
{
    class Shark
    {
        private struct Constants
        {
            public static string FILE_NAME = "shark-TgcScene.xml";
            public static float MAX_LIFE = 250;
            public static TGCVector2 SHARK_HEIGHT = new TGCVector2(700, 1800);
            public static TGCVector3 scale = new TGCVector3(5, 5, 5);
            public static TGCVector3 startPosition = TGCVector3.Empty;
            public static TGCVector3 sharkBodySize = new TGCVector3(176, 154, 560);
            public static TGCVector3 directorZ = new TGCVector3(0, 0, 1);
            public static TGCVector3 directorY = new TGCVector3(0, -1, 0);
            public static float MaxYRotation = FastMath.PI - (FastMath.QUARTER_PI / 2);
            public static float MaxAxisRotation = FastMath.QUARTER_PI;
            public static float MaxZRotation = FastMath.PI;
            public static float LIFE_REDUCE_STEP = -0.5f;
            public static float sharkMass = 1000;
            public static float SHARK_EVENT_TIME = 50;
            public static float SHARK_DEATH_TIME = 40;
        }

        private TGCVector3 director;
        private readonly Skybox skybox;
        private readonly Terrain terrain;
        private CameraFPS camera;
        private bool normalMove;
        private bool stalkerModeMove;
        private bool deathMove;
        private float acumulatedYRotation;
        private float acumulatedXRotation;
        private float acumulatedZRotation;
        private float deathTimeCounter;
        private float eventTimeCounter;
        private float DamageTaken;
        private TGCMatrix TotalRotation;
        private float Life;
        private readonly BulletRigidBodyFactory RigidBodyFactory = BulletRigidBodyFactory.Instance;
        public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);

        public RigidBody Body { get; set; }
        public TgcMesh Mesh;
        public string MediaDir { get; private set; } = Game.Default.MediaDirectory;
        public string ShadersDir { get; private set; } = Game.Default.ShadersDirectory;

        public Shark(Skybox sky, Terrain terrain, CameraFPS camera)
        {
            skybox = sky;
            this.terrain = terrain;
            this.camera = camera;
            Init();
        }

        private void Init()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + Constants.FILE_NAME).Meshes[0];
            TotalRotation = TGCMatrix.Identity;
            eventTimeCounter = Constants.SHARK_EVENT_TIME;
            deathTimeCounter = Constants.SHARK_DEATH_TIME;
            normalMove = false;
            stalkerModeMove = false;
            deathMove = false;
            acumulatedYRotation = 0;
            acumulatedXRotation = 0;
            acumulatedZRotation = 0;
            DamageTaken = 0;
            director = Constants.directorZ;
            Life = Constants.MAX_LIFE;
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * TGCMatrix.Translation(Constants.startPosition);
            Body = RigidBodyFactory.CreateBox(Constants.sharkBodySize, Constants.sharkMass, Constants.startPosition, 0, 0, 0, 0, false);
        }

        public void Update(TgcD3dInput input, float elapsedTime)
        {
            var speed = 1000f;
            var headPosition = GetHeadPosition();

            Body.ActivationState = ActivationState.ActiveTag;
            Body.AngularVelocity = Vector3.Zero;

            if (!stalkerModeMove && !normalMove && !deathMove)
                return;

            if (deathMove)
                PerformDeathMove(elapsedTime);
            else if (stalkerModeMove && CanSeekPlayer(out float rotationAngle, out TGCVector3 rotationAxis))
            {
                PerformStalkerMove(elapsedTime, speed, rotationAngle, rotationAxis);
                if (IsCollapsinWithPlayer())
                {
                    ChangeSharkWay();
                }
            }
            else if (normalMove)
                PerformNormalMove(elapsedTime, speed, headPosition);

            if (eventTimeCounter <= 0)
                ManageEndOfAttack();

            if (deathTimeCounter <= 0)
                ManageEndOfDeath();

        }

        public void Render()
        {
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * new TGCMatrix(Body.InterpolationWorldTransform);
            Mesh.BoundingBox.transform(Mesh.Transform);
            Mesh.Render();
        }

        public void Dispose()
        {
            Body.Dispose();
            Mesh.Dispose();
        }

        private void PerformNormalMove(float elapsedTime, float speed, TGCVector3 headPosition)
        {
            var XRotation = 0f;
            var YRotation = 0f;
            terrain.world.interpoledHeight(headPosition.X, headPosition.Z, out float floorHeight);
            var distanceToFloor = Body.CenterOfMassPosition.Y - floorHeight;
            var XRotationStep = FastMath.PI * 0.1f * elapsedTime;
            var YRotationStep = FastMath.PI * 0.4f * elapsedTime;

            if (distanceToFloor < Constants.SHARK_HEIGHT.X - 150 && acumulatedXRotation < Constants.MaxAxisRotation)
                XRotation = XRotationStep;
            else if (FastUtils.IsNumberBetweenInterval(distanceToFloor, Constants.SHARK_HEIGHT) && acumulatedXRotation > 0.0012)
                XRotation = -XRotationStep;
            if (distanceToFloor > Constants.SHARK_HEIGHT.Y + 150 && acumulatedXRotation > -Constants.MaxAxisRotation)
                XRotation = -XRotationStep;
            else if (FastUtils.IsNumberBetweenInterval(distanceToFloor, Constants.SHARK_HEIGHT) && acumulatedXRotation < -0.0012)
                XRotation = XRotationStep;

            if (!skybox.Contains(Body) && FastMath.Abs(acumulatedYRotation) < Constants.MaxYRotation)
                YRotation = YRotationStep * RotationYSign();
            else
                acumulatedYRotation = 0;

            acumulatedXRotation += XRotation;
            acumulatedYRotation += YRotation;

            Body.ActivationState = ActivationState.ActiveTag;
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
            Mesh.Transform = TotalRotation * TGCMatrix.Translation(new TGCVector3(Body.CenterOfMassPosition));
            Body.WorldTransform = Mesh.Transform.ToBulletMatrix();
            Body.LinearVelocity = director.ToBulletVector3() * -speed;
        }

        private void PerformStalkerMove(float elapsedTime, float speed, float rotationAngle, TGCVector3 rotationAxis)
        {
            var actualDirector = -1 * director;
            eventTimeCounter -= elapsedTime;
            var RotationStep = FastMath.PI * 0.3f * elapsedTime;

            if (rotationAngle <= RotationStep)
                return;
            actualDirector.TransformCoordinate(TGCMatrix.RotationAxis(rotationAxis, RotationStep));
            var newRotation = TGCMatrix.RotationAxis(rotationAxis, RotationStep);
            TotalRotation *= newRotation;

            Mesh.Transform = TotalRotation * TGCMatrix.Translation(new TGCVector3(Body.CenterOfMassPosition));
            Body.WorldTransform = Mesh.Transform.ToBulletMatrix();

            director = -1 * actualDirector;
            Body.LinearVelocity = director.ToBulletVector3() * -speed;
        }

        private void PerformDeathMove(float elapsedTime)
        {
            deathTimeCounter -= elapsedTime;
            var RotationStep = FastMath.PI * 0.4f * elapsedTime;
            if (acumulatedZRotation >= Constants.MaxZRotation)
                return;
            acumulatedZRotation += RotationStep;
            TotalRotation *= TGCMatrix.RotationAxis(director, RotationStep);
            Mesh.Transform = TotalRotation * TGCMatrix.Translation(new TGCVector3(Body.CenterOfMassPosition));
            Body.WorldTransform = Mesh.Transform.ToBulletMatrix();
            Body.LinearVelocity = Constants.directorY.ToBulletVector3() * 200;
        }

        public void ReceiveDamage(float damage)
        {
            DamageTaken = damage;
        }

        public bool IsDead()
        {
            return Life <= 0;
        }

        public void EndSharkAttack()
        {
            normalMove = false;
            stalkerModeMove = false;
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * TGCMatrix.Translation(Constants.startPosition);
            Body.WorldTransform = Mesh.Transform.ToBulletMatrix();
        }

        private void ManageEndOfAttack()
        {
            if (stalkerModeMove)
                ChangeSharkWay();
            stalkerModeMove = false;
            if (!skybox.Contains(Body))
                EndSharkAttack();
        }

        private void ManageEndOfDeath()
        {
            deathMove = false;
            EndSharkAttack();
        }

        private TGCVector3 CalculateInitialPosition()
        {
            var outOfSkyboxPosition = camera.Position + TGCVector3.Mul(director, new TGCVector3(0, 0, skybox.Radius + 300));
            terrain.world.interpoledHeight(outOfSkyboxPosition.X, outOfSkyboxPosition.Z, out float Y);
            return new TGCVector3(outOfSkyboxPosition.X, Y + 600, outOfSkyboxPosition.Z);
        }

        private bool IsCollapsinWithPlayer()
        {
            var distanceToPlayer = (camera.Position - GetHeadPosition()).Length();
            return distanceToPlayer < 100;
        }

        private void ChangeSharkWay()
        {
            var rotation = TGCMatrix.RotationY(FastMath.PI_HALF * -RotationYSign());
            director = Constants.directorZ;
            director.TransformCoordinate(rotation);
            Mesh.Transform = rotation * TGCMatrix.Translation(new TGCVector3(Body.CenterOfMassPosition));
            Body.WorldTransform = Mesh.Transform.ToBulletMatrix();
            acumulatedXRotation = 0;
            acumulatedYRotation = 0;
            TotalRotation = rotation;
        }

        private bool CanSeekPlayer(out float rotationAngle, out TGCVector3 rotationAxis)
        {
            var actualDirector = -1 * director;
            var directorToPlayer = TGCVector3.Normalize(camera.Position - new TGCVector3(Body.CenterOfMassPosition));
            var NormalVectorFromDirAndPlayer = FastUtils.ObtainNormalVector(actualDirector, directorToPlayer);
            if (NormalVectorFromDirAndPlayer.Length() > 0.98f)
            {
                var RotationToPlayer = FastUtils.AngleBetweenVectors(actualDirector, directorToPlayer);
                if (RotationToPlayer <= FastMath.QUARTER_PI && RotationToPlayer != 0)
                {
                    rotationAngle = RotationToPlayer;
                    rotationAxis = NormalVectorFromDirAndPlayer;
                    return true;
                }
            }
            rotationAngle = 0;
            rotationAxis = TGCVector3.Empty;
            return false;
        }

        private float RotationYSign()
        {
            var bodyToSkyboxCenterVector = TGCVector3.Normalize(skybox.GetSkyboxCenter() - new TGCVector3(Body.CenterOfMassPosition));
            var actualDirector = -1 * director;
            var normalVector = TGCVector3.Cross(actualDirector, bodyToSkyboxCenterVector);
            return normalVector.Y > 0 ? 1 : -1;
        }

        private TGCVector3 GetHeadPosition()
        {
            return new TGCVector3(Body.CenterOfMassPosition) + director * -560;
        }
    }
}
