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

        public TGCVector3 Position { get; private set; }

        public CharacterRigidBody(CameraFPS camera)
        {
            Camera = camera;
        //  Camera.isOutside = true;
        }

        public override void Init()
        {
            if (Camera.isOutside)
                Position = Camera.getShipOutsidePosition();
            else
                Position = Camera.getShipInsidePosition();

            rigidBody = rigidBodyFactory.CreateBall(30f, 0.75f, Position);
        }

        public override void Update(TgcD3dInput input)
        {
            var strength = 5f;
            rigidBody.ActivationState = ActivationState.ActiveTag;
            // TODO: Corregir el movimiento ya que ahora hace cualquiera
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

            if (Camera.isOutside)
                rigidBody.Gravity = new Vector3(0, -10, 0); 
            else
                rigidBody.Gravity = new Vector3(0, 0, 0);

            Camera.position = new TGCVector3(rigidBody.CenterOfMassPosition.X,
                                             rigidBody.CenterOfMassPosition.Y,
                                             rigidBody.CenterOfMassPosition.Z);
        }

        public override void Dispose()
        {
            rigidBody.Dispose();
        }
    }
}
