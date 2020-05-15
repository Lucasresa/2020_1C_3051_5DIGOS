using BulletSharp;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class OutdoorShipRigidBody
    {
        #region Atributos
        struct Constants
        {
            public static TGCVector3 scale = new TGCVector3(10, 10, 10);
            public static TGCVector3 position = new TGCVector3(530, 3630, 100);
        }

        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        private TgcMesh Mesh;
        public RigidBody body;
        #endregion

        #region Constructor
        public OutdoorShipRigidBody(Ship ship)
        {
            Mesh = ship.OutdoorMesh;
            Init();
        }
        #endregion

        #region Metodos
        private void Init()
        {
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(Constants.position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(Constants.position)).ToBulletMatrix();
            body.CollisionShape.LocalScaling = Constants.scale.ToBulletVector3();
            Mesh.BoundingBox.scaleTranslate(Constants.position, Constants.scale);
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

        public TgcBoundingAxisAlignBox getAABB()
        {
            return Mesh.BoundingBox;
        }
        #endregion
    }
}
