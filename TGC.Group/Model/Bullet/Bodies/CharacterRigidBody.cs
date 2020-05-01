using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    // INFO: Por ahora voy a diseñar el personaje como una sphere que represente a la camara, pero estaba viendo que hay que utilizar una capsula para el personaje.. despues modificare
    class CharacterRigidBody : RigidBodies
    {
        private CameraFPS Camera;
        private TGCVector3 directorz = new TGCVector3(1, 0, 0); 
        private TGCVector3 directorx = new TGCVector3(0, 0, 1);

        public TGCVector3 Position { get; private set; }

        public CharacterRigidBody(CameraFPS camera)
        {
            Camera = camera;
        }

        public override void Init()
        {            
            if (Camera.isOutside)
                Position = Camera.getShipOutsidePosition();
            else
                Position = Camera.getShipInsidePosition();

            RigidBody = rigidBodyFactory.CreateCapsule(15, 30, Position, 10, false);
        }

        public override void Update(TgcD3dInput input)
        {
            var strength = 2f;

            RigidBody.ActivationState = ActivationState.ActiveTag;

            // TODO: Corregir el movimiento ya que ahora hace cualquiera
            #region Movimiento 
            RigidBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();

            if (input.keyDown(Key.W))
            {
                RigidBody.ApplyCentralImpulse(-strength * directorz.ToBulletVector3());
            }

            if (input.keyDown(Key.S))
            {
                RigidBody.ApplyCentralImpulse(strength * directorz.ToBulletVector3());
            }

            if (input.keyDown(Key.A))
            {
                RigidBody.ApplyCentralImpulse(-strength * directorx.ToBulletVector3());
            }

            if (input.keyDown(Key.D))
            {
                RigidBody.ApplyCentralImpulse(strength * directorx.ToBulletVector3());
            }

            if (input.keyPressed(Key.Space))
            {                
                RigidBody.ApplyCentralImpulse(new TGCVector3(0, 80 * strength, 0).ToBulletVector3());
            }

            #endregion

            if (Camera.isOutside)
                RigidBody.Gravity = new Vector3(0, -10, 0); // INFO: Cambiar a 0 cuando se deje de probar afuera
            else
                RigidBody.Gravity = new Vector3(0, 0, 0);

            Camera.position = new TGCVector3(RigidBody.CenterOfMassPosition.X,
                                             RigidBody.CenterOfMassPosition.Y,
                                             RigidBody.CenterOfMassPosition.Z);
        }

        public override void Render()
        {
            
        }

        public override void Dispose()
        {
            RigidBody.Dispose();
        }
    }
}
