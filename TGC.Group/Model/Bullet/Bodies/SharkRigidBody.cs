using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Sharky;

namespace TGC.Group.Model.Bullet.Bodies
{
    class SharkRigidBody : RigidBody
    {
        #region Atributos
        private TGCVector3 scale = new TGCVector3(5, 5, 5);
        private TGCVector3 position = new TGCVector3(-2885, 1320, -525);
        private TGCVector3 director = new TGCVector3(0, 0, 1);
        private TGCVector3 prevPosition;
        private TGCMatrix rotation = TGCMatrix.Identity;
        #endregion

        #region Constructor

        public SharkRigidBody(Shark shark)
        {
            this.mesh = shark.Mesh;
            prevPosition = position;
        }

        #endregion

        #region Metodos

        public override void Init()
        {
            Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.SetMassProps(1, new Vector3(1, 1, 1));
            body.CollisionShape.LocalScaling = scale.ToBulletVector3();
            body.CenterOfMassTransform = TGCMatrix.Translation(position).ToBulletMatrix();
        }

        public override void Update(TgcD3dInput input, float elapsedTime)
        {
            var angle = 5;

            if (input.keyDown(Key.UpArrow))
            {
                //Activa el comportamiento de la simulacion fisica para la capsula
                body.ActivationState = ActivationState.ActiveTag;
                body.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                body.ApplyCentralImpulse(-100 * director.ToBulletVector3());
            }

            if (input.keyDown(Key.DownArrow))
            {
                //Activa el comportamiento de la simulacion fisica para la capsula
                body.ActivationState = ActivationState.ActiveTag;
                body.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                body.ApplyCentralImpulse(100 * director.ToBulletVector3());
            }

            if (input.keyDown(Key.LeftArrow))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.01f));
                mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(-angle * 0.01f) * new TGCMatrix(body.InterpolationWorldTransform);
                body.WorldTransform = mesh.Transform.ToBulletMatrix();
            }

            if (input.keyDown(Key.RightArrow))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.01f));
                mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(angle * 0.01f) * new TGCMatrix(body.InterpolationWorldTransform);
                body.WorldTransform = mesh.Transform.ToBulletMatrix();
            }

            if (input.keyDown(Key.P))
            {
                body.LinearVelocity = Vector3.Zero;
                body.CenterOfMassTransform = TGCMatrix.Translation(position).ToBulletMatrix();
            }

        }

        public override void Render()
        {
            mesh.Transform = TGCMatrix.Scaling(scale) * new TGCMatrix(body.InterpolationWorldTransform) * TGCMatrix.Translation(new TGCVector3(0, -35, 0));
            mesh.Render();
        }

        public override void Dispose()
        {
            body.Dispose();
            mesh.Dispose();
        }

        #endregion
    }
}
