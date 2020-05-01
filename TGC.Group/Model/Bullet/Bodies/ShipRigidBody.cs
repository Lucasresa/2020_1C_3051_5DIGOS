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
using TGC.Core.Interpolation;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Textures;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class ShipRigidBody : RigidBodies
    {
        #region Atributos
        public RigidBodyType Type { get; }
        public Ship Ship;
        #endregion

        #region Constructor
        public ShipRigidBody(RigidBodyType type, Ship ship)
        {
            Type = type;
            Ship = ship;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            switch (Type)
            {
                case RigidBodyType.indoor:
                    RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.IndoorMesh);
                    RigidBody.Translate(Ship.IndoorMesh.Position.ToBulletVector3());
                    break;
                case RigidBodyType.outdoor:
                    RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.OutdoorMesh);
                    RigidBody.Translate(Ship.OutdoorMesh.Position.ToBulletVector3());
                    break;
            }
            RigidBody.CollisionShape.LocalScaling = new Vector3(10, 10, 10);
        }

        public override void Render()
        {
            Ship.Render();
        }

        public override void Update(TgcD3dInput input)
        {
            RigidBody.ActivationState = ActivationState.ActiveTag;

            switch (Type)
            {
                case RigidBodyType.indoor:
                    Ship.IndoorMesh.Transform = TGCMatrix.Scaling(10, 10, 10) * TGCMatrix.Translation(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y, RigidBody.CenterOfMassPosition.Z);
                    break;
                case RigidBodyType.outdoor:
                    Ship.OutdoorMesh.Transform = TGCMatrix.Scaling(10, 10, 10) * TGCMatrix.Translation(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y, RigidBody.CenterOfMassPosition.Z);
                    break;
            }
        }

        public override void Dispose()
        {
            RigidBody.Dispose();
            Ship.Dispose();
        }
        #endregion
    }
}
