﻿using BulletSharp.Math;
using System;
using System.Collections.Generic;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Watercraft;
using TGC.Group.Utils;
using BTRigidBody = BulletSharp.RigidBody;

namespace TGC.Group.Model.Bullet
{
    class RigidBody
    {
        #region Atributos

        protected BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        public BTRigidBody body;
        private List<RigidBody> rigidBodies = new List<RigidBody>();
        public bool isTerrain = false;
        public bool isIndoorShip = false;
        protected TgcMesh mesh;
        public bool isCharacter = false;
        public TgcMesh Mesh { get { return mesh; } }
        #endregion

        #region Metodos
        public void Initializer(Terrain terrain, CameraFPS camera, Shark shark, Ship ship, List<TgcMesh> meshes, Sky skybox)
        {
            rigidBodies.Add(new TerrainRigidBody(terrain));
            rigidBodies.Add(new CharacterRigidBody(camera));
            rigidBodies.Add(new SharkRigidBody(shark, skybox, terrain, camera));
            rigidBodies.Add(new OutdoorShipRigidBody(ship));
            rigidBodies.Add(new IndoorShipRigidBody(ship));
            meshes.ForEach(mesh => rigidBodies.Add(new CommonRigidBody(mesh)));
            meshes.RemoveRange(0, meshes.Count);
            rigidBodies.ForEach(rigidBody => rigidBody.Init());
        }
        
        public virtual void Init() { }
        public virtual void Teleport() { }
        public virtual void Render() { }
        public virtual void Dispose() { }
        public virtual void Update(TgcD3dInput input, float elapsedTime) { }

        public List<RigidBody> getListRigidBody()
        {
            return rigidBodies;
        }

        #endregion
    }
}