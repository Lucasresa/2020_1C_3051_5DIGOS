using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    class CharacterRigidBody : RigidBody
    {
        private CameraFPS Camera;
        private TGCVector3 directorz = new TGCVector3(1, 0, 0);
        private TGCVector3 directorx = new TGCVector3(0, 0, 1);
        private TGCVector3 position;
        private TGCVector3 indoorPosition;
        private TGCVector3 outdoorPosition;

        public CharacterRigidBody(CameraFPS camera)
        {
            Camera = camera;
            indoorPosition = camera.getIndoorPosition();
            outdoorPosition = camera.getOutdoorPosition();
        }

        public override void Init()
        {
            if (Camera.isOutside) position = Camera.getOutdoorPosition();
            else position = Camera.getIndoorPosition();

            body = rigidBodyFactory.CreateBall(30f, 0.75f, position);
            body.CenterOfMassTransform = TGCMatrix.Translation(position).ToBulletMatrix();
        }

        public override void Update(TgcD3dInput input, float elapsedTime)
        {
            var strength = 5f;
            body.ActivationState = ActivationState.ActiveTag;
        
            #region Movimiento 
            body.AngularVelocity = TGCVector3.Empty.ToBulletVector3();

            if (input.keyDown(Key.W))
            {
                body.ApplyCentralImpulse(-strength * directorz.ToBulletVector3());
            }

            if (input.keyDown(Key.S))
            {
                body.ApplyCentralImpulse(strength * directorz.ToBulletVector3());
            }

            if (input.keyDown(Key.A))
            {
                body.ApplyCentralImpulse(-strength * directorx.ToBulletVector3());
            }

            if (input.keyDown(Key.D))
            {
                body.ApplyCentralImpulse(strength * directorx.ToBulletVector3());
            }

            if (input.keyPressed(Key.Space))
            {
                body.ApplyCentralImpulse(new TGCVector3(0, 80 * strength, 0).ToBulletVector3());
            }

            if (input.keyPressed(Key.LeftControl))
            {
                body.ApplyCentralImpulse(new TGCVector3(0, 80 * -strength, 0).ToBulletVector3());
            }

            #endregion

            Camera.position = new TGCVector3(body.CenterOfMassPosition);

            #region Teclas

            if (input.keyPressed(Key.E)) Teleport();

            #endregion
        }

        public override void Dispose()
        {
            body.Dispose();
        }

        public override void Teleport()
        {
            if (Camera.isOutside)
                body.CenterOfMassTransform = TGCMatrix.Translation(indoorPosition).ToBulletMatrix();
            else 
                body.CenterOfMassTransform = TGCMatrix.Translation(outdoorPosition).ToBulletMatrix();

            Camera.position = new TGCVector3(body.CenterOfMassPosition);
            body.LinearVelocity = Vector3.Zero;
            body.AngularVelocity = Vector3.Zero;
        }
    }
}
