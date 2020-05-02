using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class IndoorShipRigidBody : RigidBody
    {
        #region Atributos
        public Ship Ship;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);
        private TGCVector3 position = new TGCVector3(350, -2500, -45);
        #endregion

        #region Constructor
        public IndoorShipRigidBody(Ship ship)
        {
            Ship = ship;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            Ship.IndoorMesh.Transform = TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
            rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.IndoorMesh);
            rigidBody.CollisionShape.LocalScaling = scale.ToBulletVector3();
            rigidBody.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
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
