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
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        private TGCVector3 position = new TGCVector3(350, -2500, -45);
        private TGCVector3 scale = new TGCVector3(20, 20, 20);
        public TgcMesh Mesh;
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
            Mesh.Transform = TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CollisionShape.LocalScaling = scale.ToBulletVector3();
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
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
