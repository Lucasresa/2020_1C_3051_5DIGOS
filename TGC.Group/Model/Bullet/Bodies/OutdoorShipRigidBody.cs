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
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);
        private TGCVector3 position = new TGCVector3(530, 3630, 100);
        public TgcMesh Mesh;
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
            Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
            body.CollisionShape.LocalScaling = scale.ToBulletVector3();
            Mesh.BoundingBox.scaleTranslate(position, scale);
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
