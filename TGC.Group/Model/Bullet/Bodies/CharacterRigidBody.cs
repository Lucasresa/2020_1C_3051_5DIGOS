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
        private TGCVector3 director = new TGCVector3(0, 0, 1);

        public CharacterRigidBody(RigidBodyType type, CameraFPS camera)
        {
            Type = type;
            Camera = camera;
        }

        public override void Init()
        {
            switch (Type)
            {
                case RigidBodyType.insideCharacter:
                    Position = Camera.getShipInsidePosition();
                    break;
                case RigidBodyType.outsideCharacter:
                    Position = Camera.getShipOutsidePosition();
                    break;
                default:
                    throw new Exception("Type not found");
            }
            
            RigidBody = rigidBodyFactory.CreateBall(30f, 0.75f, Position);            
        }

        public override void Update(TgcD3dInput input)
        {
            var strength = 10.30f;
            var angle = 5;

            RigidBody.ActivationState = ActivationState.ActiveTag;

            if (input.keyDown(Key.W))
            {
                RigidBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                RigidBody.ApplyCentralImpulse(-strength * director.ToBulletVector3());
            }

            if (input.keyDown(Key.S))
            {
                RigidBody.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                RigidBody.ApplyCentralImpulse(strength * director.ToBulletVector3());
            }

            if (input.keyDown(Key.A))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.01f));
            }

            if (input.keyDown(Key.D))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.01f));
            }

            if (input.keyPressed(Key.Space))
            {                
                RigidBody.ApplyCentralImpulse(new TGCVector3(0, 80 * strength, 0).ToBulletVector3());
            }

            RigidBody.Gravity = new Vector3(0, -100, 0);

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
