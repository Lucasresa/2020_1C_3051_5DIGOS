using BulletSharp;
using BulletSharp.Math;
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
            rigidBody.CollisionShape.LocalScaling = new Vector3(3, 3, 3);
        }

        public override void Update(TgcD3dInput input)
        {
            Sharky.Mesh.Transform = TGCMatrix.Scaling(3, 3, 3) * TGCMatrix.Translation(rigidBody.CenterOfMassPosition.X, rigidBody.CenterOfMassPosition.Y, rigidBody.CenterOfMassPosition.Z);
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
