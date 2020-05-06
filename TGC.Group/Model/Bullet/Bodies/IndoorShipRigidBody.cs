using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class IndoorShipRigidBody : RigidBody
    {
        #region Atributos
        public TgcMesh Mesh;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);
        private TGCVector3 position = new TGCVector3(350, -2500, -45);
        #endregion

        #region Constructor
        public IndoorShipRigidBody(Ship ship)
        {
            Mesh = ship.IndoorMesh;
            isIndoorShip = true;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            Mesh.Transform = TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CollisionShape.LocalScaling = scale.ToBulletVector3();
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
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

        public override TgcMesh getMesh()
        {
            return Mesh;
        }
        #endregion
    }
}
