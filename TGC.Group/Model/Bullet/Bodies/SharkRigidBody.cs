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
using TGC.Group.Model.Sharky;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    class SharkRigidBody : RigidBody
    {
        private Shark Sharky;

        public SharkRigidBody(Shark shark)
        {
            Sharky = shark;
            Sharky.LoadShark();
        }

        public override void Init()
        {
            rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Sharky.Mesh);
            rigidBody.Translate(Sharky.Mesh.Position.ToBulletVector3());
            rigidBody.CollisionShape.LocalScaling = new Vector3(5, 5, 5);
            rigidBody.CenterOfMassTransform = TGCMatrix.Translation(Sharky.Mesh.Position).ToBulletMatrix();
        }

        public override void Update(TgcD3dInput input)
        {
            if (input.keyPressed(Key.Z))
            {
                rigidBody.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(-300,1500,360)).ToBulletMatrix();
            }

            Sharky.Mesh.Transform = TGCMatrix.Scaling(5, 5, 5) * new TGCMatrix(rigidBody.InterpolationWorldTransform);
        }

        public override void Render()
        {
            Sharky.Render();
        }

        public override void Dispose()
        {
            rigidBody.Dispose();
            Sharky.Dispose();
        }
    }
}
