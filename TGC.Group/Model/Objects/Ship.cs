using BulletSharp;
using BulletSharp.Math;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Objects
{
    class Ship
    {
        struct Constants
        {
            public static string FILE_NAME = "ship-TgcScene.xml";
            public static TGCVector3 PositionIndoorShip = new TGCVector3(515, -2340, -40);
            public static TGCVector3 PositionOutdoorShip = new TGCVector3(530, 3630, 100);
            public static TGCVector3 Rotation = new TGCVector3(FastMath.PI_HALF, 0, 0);
            public static TGCVector3 Scale = new TGCVector3(10, 10, 10);
        }

        public TgcMesh OutdoorMesh, IndoorMesh;
        public RigidBody BodyIndoorShip;
        public RigidBody BodyOutdoorShip;

        private string MediaDir, ShadersDir;
        private readonly BulletRigidBodyFactory RigidBodyFactory = BulletRigidBodyFactory.Instance;

        public Ship(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Init();
        }

        public void Dispose()
        {
            OutdoorMesh.Dispose();
            IndoorMesh.Dispose();
            BodyIndoorShip.Dispose();
            BodyOutdoorShip.Dispose();
        }

        public void Init()
        {
            LoadShip();
            TransformMesh(OutdoorMesh, Constants.PositionOutdoorShip, Constants.Scale, Constants.Rotation);
            TransformMesh(IndoorMesh, Constants.PositionIndoorShip, Constants.Scale, Constants.Rotation);
            BodyOutdoorShip = TransformRigidBody(OutdoorMesh, Constants.PositionOutdoorShip, Constants.Scale, Constants.Rotation);
            BodyIndoorShip = TransformRigidBody(IndoorMesh, Constants.PositionIndoorShip, Constants.Scale, Constants.Rotation);
        }

        private void LoadShip()
        {
            OutdoorMesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + Constants.FILE_NAME).Meshes[0];
            IndoorMesh = OutdoorMesh.createMeshInstance("InsideRoom");
        }

        private void TransformMesh(TgcMesh mesh, TGCVector3 position, TGCVector3 scale, TGCVector3 rotation)
        {
            mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * TGCMatrix.Translation(position);
            mesh.BoundingBox.scaleTranslate(position, scale);
        }

        private RigidBody TransformRigidBody(TgcMesh mesh, TGCVector3 position, TGCVector3 scale, TGCVector3 rotation)
        {
            var rigidBody = RigidBodyFactory.CreateRigidBodyFromTgcMesh(mesh);
            rigidBody.CenterOfMassTransform = Matrix.RotationYawPitchRoll(rotation.X, rotation.Y, rotation.Z) * Matrix.Translation(position.ToBulletVector3());
            rigidBody.CollisionShape.LocalScaling = scale.ToBulletVector3();
            return rigidBody;
        }

        public void Render()
        {
            IndoorMesh.Render();
            OutdoorMesh.Render();
        }
    }
}
