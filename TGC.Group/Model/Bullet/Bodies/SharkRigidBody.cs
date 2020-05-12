using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Sharky;

namespace TGC.Group.Model.Bullet.Bodies
{
    class SharkRigidBody
    {
        #region Atributos
        struct Constants
        {
            public static TGCVector3 scale = new TGCVector3(5, 5, 5);
            public static TGCVector3 position = new TGCVector3(-2885, 1220, -525);
        }
        
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;

        public RigidBody body;
        public TgcMesh Mesh;
        
        #endregion

        #region Constructor

        public SharkRigidBody(Shark shark)
        {
            Mesh = shark.Mesh;
            Init();
        }

        #endregion

        #region Metodos

        private void Init()
        {
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * TGCMatrix.RotationYawPitchRoll(-FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(Constants.position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.SetMassProps(1, new Vector3(1, 1, 1));
            body.CollisionShape.LocalScaling = Constants.scale.ToBulletVector3();
            body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(-FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(Constants.position)).ToBulletMatrix();
        }

        public void Update(TgcD3dInput input, float elapsedTime)
        {
            if (input.keyPressed(Key.Z))
            {
                body.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(-300, 1500, 360)).ToBulletMatrix();
            }

            if (input.keyPressed(Key.M))
            {
                body.ApplyCentralImpulse(new Vector3(10, 0, 0));
            }
            Mesh.Transform = TGCMatrix.Scaling(Constants.scale) * new TGCMatrix(body.InterpolationWorldTransform);
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
