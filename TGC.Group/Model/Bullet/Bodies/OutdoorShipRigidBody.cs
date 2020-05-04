using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class OutdoorShipRigidBody : RigidBody
    {
        #region Atributos
        public TgcMesh Mesh;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);
        private TGCVector3 position = new TGCVector3(530, 3630, 100);
        #endregion

        #region Constructor
        public OutdoorShipRigidBody(Ship ship)
        {
            Mesh = ship.OutdoorMesh;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
            body.CollisionShape.LocalScaling = scale.ToBulletVector3();
        }

        public override void Render()
        {
            Mesh.Render();
        }

        public override void Dispose()
        {
            body.Dispose();
            Mesh.Dispose();
        }
        #endregion
    }
}
