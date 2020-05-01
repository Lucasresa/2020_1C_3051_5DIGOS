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
    class SharkRigidBody : RigidBodies
    {
        private Shark Sharky;

        public SharkRigidBody(Shark shark)
        {
            Sharky = shark;
            Sharky.LoadShark();
        }

        public override void Init()
        {
            RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Sharky.Mesh);
            RigidBody.Translate(Sharky.Mesh.Position.ToBulletVector3());
            RigidBody.CollisionShape.LocalScaling = new Vector3(3, 3, 3);
        }

        public override void Update(TgcD3dInput input)
        {
            //RigidBody.ActivationState = ActivationState.ActiveTag;
        }

        public override void Render()
        {
            Sharky.Mesh.Transform = TGCMatrix.Scaling(3, 3, 3) * TGCMatrix.Translation(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y, RigidBody.CenterOfMassPosition.Z);
            Sharky.Render();
        }

        public override void Dispose()
        {
            RigidBody.Dispose();
            Sharky.Dispose();
        }
    }
}
