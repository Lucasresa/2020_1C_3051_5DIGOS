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
        #region Atributos
        private Shark Sharky;
        private TGCVector3 scale = new TGCVector3(5, 5, 5);
        private TGCVector3 position = new TGCVector3(-2885, 1220, -525);
        #endregion

        #region Constructor

        public SharkRigidBody(Shark shark)
        {
            Sharky = shark;
        }

        #endregion

        #region Metodos

        public override void Init()
        {
            Sharky.Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.RotationYawPitchRoll(-FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position);
            rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Sharky.Mesh);
            rigidBody.SetMassProps(1, new Vector3(1, 1, 1));                   
            rigidBody.CollisionShape.LocalScaling = scale.ToBulletVector3();
            rigidBody.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(-FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(position)).ToBulletMatrix();
        }

        public override void Update(TgcD3dInput input)
        {
            if (input.keyPressed(Key.Z))
            {
                rigidBody.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(-300, 1500, 360)).ToBulletMatrix();
            }

            if (input.keyPressed(Key.M))
            {
                rigidBody.ApplyCentralImpulse(new Vector3(10, 0, 0));
            }
            Sharky.Mesh.Transform = TGCMatrix.Scaling(scale) * new TGCMatrix(rigidBody.InterpolationWorldTransform);
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

        #endregion
    }
}
