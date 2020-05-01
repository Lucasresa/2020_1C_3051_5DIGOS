﻿using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Watercraft;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet
{
    class RigidBodies
    {
        #region Atributos

        protected BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        public Dictionary<RigidBodyType, RigidBodies> rigidBodies = new Dictionary<RigidBodyType, RigidBodies>();

        public RigidBody RigidBody;

        #endregion

        #region Constructor
        public RigidBodies() { }
        #endregion

        #region Metodos
        public void Initializer(Terrain terrain, CameraFPS camera, Shark shark, Ship ship)
        {
            rigidBodies.Add(RigidBodyType.terrain, new TerrainRigidBody(terrain));
            rigidBodies.Add(RigidBodyType.character, new CharacterRigidBody(camera));
            rigidBodies.Add(RigidBodyType.shark, new SharkRigidBody(shark));
            rigidBodies.Add(RigidBodyType.outdoor, new ShipRigidBody(RigidBodyType.outdoor, ship));
            rigidBodies.Add(RigidBodyType.indoor, new ShipRigidBody(RigidBodyType.indoor, ship));

            rigidBodies.Values.ToList().ForEach(rigidBody => rigidBody.Init());
        }

        public virtual void Init() { }
        public virtual void Update(TgcD3dInput input) { }

        public virtual void Render()
        {
            rigidBodies.Values.ToList().ForEach(rigidBody => rigidBody.Render());
        }

        public virtual void Dispose()
        {
            rigidBodies.Values.ToList().ForEach(rigidBody => rigidBody.Dispose());
        }        

        public void setGravity(RigidBody rigidBody, float gravity)
        {
            rigidBody.Gravity = new Vector3(0, -gravity, 0);
        }
        
        #endregion
    }
}
