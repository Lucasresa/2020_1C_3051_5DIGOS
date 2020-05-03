using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class IndoorShipRigidBody : RigidBody
    {
        #region Atributos
        public TgcMesh indoorMesh;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);
        private TGCVector3 position = new TGCVector3(350, -2500, -45);
        #endregion

        #region Constructor
        public IndoorShipRigidBody(Ship ship)
        {
            indoorMesh = ship.IndoorMesh;
            isIndoorShip = true;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            indoorMesh.Transform = TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
            rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(indoorMesh);
            rigidBody.CollisionShape.LocalScaling = scale.ToBulletVector3();
            rigidBody.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
        }

        public override void Render()
        {
            indoorMesh.Render();
        }

        public override void Dispose()
        {
            rigidBody.Dispose();
            indoorMesh.Dispose();
        }
        #endregion
    }
}
