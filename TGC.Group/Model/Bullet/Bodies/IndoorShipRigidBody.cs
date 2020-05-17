using BulletSharp;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class IndoorShipRigidBody
    {
        #region Atributos
        struct Constants
        {
            public static TGCVector3 position = new TGCVector3(515, -2340, -40);
            public static TGCVector3 scale = new TGCVector3(10, 10, 10);
        }
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        private TgcMesh Mesh;
        public RigidBody body;
        #endregion

        #region Constructor
        public IndoorShipRigidBody(Ship ship)
        {
            Mesh = ship.IndoorMesh;
            Init();
        }
        #endregion

        #region Metodos
        private void Init()
        {
            Mesh.Transform = TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Scaling(Constants.scale) * TGCMatrix.Translation(Constants.position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CollisionShape.LocalScaling = Constants.scale.ToBulletVector3();
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(Constants.position)).ToBulletMatrix();
        }

        public void Render()
        {
            Mesh.Render();
        }

        public void Dispose()
        {
            body.Dispose();
            Mesh.Dispose();
        }
        #endregion
    }
}
