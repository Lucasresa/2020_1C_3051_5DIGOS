using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Sharky;

namespace TGC.Group.Model.Bullet.Bodies
{
    class SharkRigidBody : RigidBody
    {
        #region Atributos
        private TgcMesh Mesh;
        private TGCVector3 scale = new TGCVector3(5, 5, 5);
        private TGCVector3 position = new TGCVector3(-2885, 1220, -525);
        #endregion

        #region Constructor

        public SharkRigidBody(Shark shark)
        {
            Mesh = shark.Mesh;
        }

        #endregion

        #region Metodos

        public override void Init()
        {
            Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(-FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.SetMassProps(1, new Vector3(1, 1, 1));
            body.CollisionShape.LocalScaling = scale.ToBulletVector3();
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(-FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
        }

        public override void Update(TgcD3dInput input, float elapsedTime)
        {
            if (input.keyPressed(Key.Z))
            {
                body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(-300, 1500, 360)).ToBulletMatrix();
            }

            if (input.keyPressed(Key.M))
            {
                body.ApplyCentralImpulse(new Vector3(10, 0, 0));
            }
            Mesh.Transform = TGCMatrix.Scaling(scale) * new TGCMatrix(body.InterpolationWorldTransform);
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
