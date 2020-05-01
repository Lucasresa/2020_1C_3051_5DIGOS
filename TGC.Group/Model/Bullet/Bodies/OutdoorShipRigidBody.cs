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
    class OutdoorShipRigidBody : RigidBody
    {
        #region Atributos
        public Ship Ship;
        #endregion

        #region Constructor
        public OutdoorShipRigidBody(Ship ship)
        {
            Ship = ship;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.OutdoorMesh);
            rigidBody.Translate(Ship.OutdoorMesh.Position.ToBulletVector3());
            rigidBody.CollisionShape.LocalScaling = new Vector3(10, 10, 10);
        }

        public override void Render()
        {
            Ship.Render();
        }

        public override void Update(TgcD3dInput input)
        {
            rigidBody.ActivationState = ActivationState.ActiveTag;
            Ship.OutdoorMesh.Transform = TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Scaling(10, 10, 10) * TGCMatrix.Translation(rigidBody.CenterOfMassPosition.X, rigidBody.CenterOfMassPosition.Y, rigidBody.CenterOfMassPosition.Z);
            rigidBody.CenterOfMassTransform = (TGCMatrix.RotationYawPitchRoll(FastMath.PI_HALF, 0, 0) * TGCMatrix.Translation(Ship.OutdoorMesh.Position)).ToBulletMatrix();
        }

        public override void Dispose()
        {
            rigidBody.Dispose();
            Ship.Dispose();
        }
        #endregion
    }
}
