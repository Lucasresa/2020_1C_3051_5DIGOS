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
            // Camera.isOutside = true; // TODO: Descomentar para salir afuera
            indoorPosition = camera.getShipInsidePosition();
            outdoorPosition = camera.getShipOutsidePosition();
        }

        public override void Init()
        {
            if (Camera.isOutside) position = Camera.getShipOutsidePosition();
            else position = Camera.getShipInsidePosition();

            rigidBody = rigidBodyFactory.CreateBall(30f, 0.75f, position);
            rigidBody.CenterOfMassTransform = TGCMatrix.Translation(position).ToBulletMatrix();
        }

        public override void Update(TgcD3dInput input)
        {
            var strength = 5f;
            rigidBody.ActivationState = ActivationState.ActiveTag;
        
            #region Movimiento 
            rigidBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();

            if (input.keyDown(Key.W))
            {
                rigidBody.ApplyCentralImpulse(-strength * directorz.ToBulletVector3());
            }

            if (input.keyDown(Key.S))
            {
                rigidBody.ApplyCentralImpulse(strength * directorz.ToBulletVector3());
            }

            if (input.keyDown(Key.A))
            {
                rigidBody.ApplyCentralImpulse(-strength * directorx.ToBulletVector3());
            }

            if (input.keyDown(Key.D))
            {
                rigidBody.ApplyCentralImpulse(strength * directorx.ToBulletVector3());
            }

            if (input.keyPressed(Key.Space))
            {
                rigidBody.ApplyCentralImpulse(new TGCVector3(0, 80 * strength, 0).ToBulletVector3());
            }

            if (input.keyPressed(Key.LeftControl))
            {
                rigidBody.ApplyCentralImpulse(new TGCVector3(0, 80 * -strength, 0).ToBulletVector3());
            }

            #endregion

            Camera.position = new TGCVector3(rigidBody.CenterOfMassPosition);

            #region Teclas

            if (input.keyPressed(Key.E)) Teleport();

            #endregion
        }

        public override void Dispose()
        {
            rigidBody.Dispose();
        }

        public override void Teleport()
        {
            if (Camera.isOutside)
                rigidBody.CenterOfMassTransform = TGCMatrix.Translation(indoorPosition).ToBulletMatrix();
            else 
                rigidBody.CenterOfMassTransform = TGCMatrix.Translation(outdoorPosition).ToBulletMatrix();

            Camera.position = new TGCVector3(rigidBody.CenterOfMassPosition);
            rigidBody.LinearVelocity = Vector3.Zero;
            rigidBody.AngularVelocity = Vector3.Zero;
        }
    }
}
