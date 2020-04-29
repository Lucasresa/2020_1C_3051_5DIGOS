using BulletSharp;
using BulletSharp.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Geometry;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class ShipRigidBody : RigidBodies
    {
        public RigidBodyType Type { get; }
        public Ship Ship;

        public ShipRigidBody(RigidBodyType type, Ship ship)
        {
            Type = type;
            Ship = ship;
        }
        
        public override void Init()
        {            
            switch (Type)
            {
                case RigidBodyType.insideShip:
                    RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.InsideMesh);
                    break;
                case RigidBodyType.outsideShip:
                    RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.Mesh);
                    break;
            }
            RigidBody.Translate(Ship.Mesh.Position.ToBulletVector3());
        }

        public override void Render()
        {
            Ship.Mesh.Transform = TGCMatrix.Scaling(1, 1, 1) * new TGCMatrix(RigidBody.InterpolationWorldTransform);
            Ship.Render();
        }

        // INFO: ESTAN OK
        #region Metodos
        public override void Update(TgcD3dInput input)
        {
            RigidBody.ActivationState = ActivationState.ActiveTag;
        }

        public override void Dispose()
        {
            RigidBody.Dispose();
            Ship.Dispose();
        }
        #endregion

    }
}
