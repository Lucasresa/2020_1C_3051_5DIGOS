using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class OutdoorShipRigidBody : RigidBody
    {
        #region Atributos
        public Ship Ship;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);
        private TGCVector3 position = new TGCVector3(530, 3630, 100);
        #endregion

        #region Constructor
        public OutdoorShipRigidBody(Ship ship)
        {
            Ship = ship;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            Ship.OutdoorMesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position);
            rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.OutdoorMesh);
            rigidBody.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
            rigidBody.CollisionShape.LocalScaling = scale.ToBulletVector3();
        }

        public override void Render()
        {
            Ship.Render();
        }

        public override void Dispose()
        {
            rigidBody.Dispose();
            Ship.Dispose();
        }
        #endregion
    }
}
