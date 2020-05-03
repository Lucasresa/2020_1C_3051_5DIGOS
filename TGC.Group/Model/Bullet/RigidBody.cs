using BulletSharp.Math;
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
        public BTRigidBody rigidBody;
        public List<RigidBody> rigidBodies = new List<RigidBody>();

        #endregion

        #region Constructor
        public RigidBody() { }
        #endregion

        #region Metodos
        public void Initializer(Terrain terrain, CameraFPS camera, Shark shark, Ship ship, List<TgcMesh> meshes)
        {
            rigidBodies.Add(new TerrainRigidBody(terrain));
            rigidBodies.Add(new CharacterRigidBody(camera));
            rigidBodies.Add(new SharkRigidBody(shark));
            rigidBodies.Add(new OutdoorShipRigidBody(ship));
            rigidBodies.Add(new IndoorShipRigidBody(ship));
            meshes.ForEach(mesh => rigidBodies.Add(new CommonRigidBody(mesh)));

            rigidBodies.ForEach(rigidBody => rigidBody.Init());
        }

        public virtual void Init() { }
        public virtual void Teleport() { }
        public virtual void Update(TgcD3dInput input) { }

        public virtual void Render()
        {
            rigidBodies.ForEach(rigidBody => rigidBody.Render());
        }

        public virtual void Dispose()
        {
            rigidBodies.ForEach(rigidBody => rigidBody.Dispose());
        }

        public void setGravity(BTRigidBody rigidBody, float gravity)
        {
            rigidBody.Gravity = new Vector3(0, -gravity, 0);
        }

        #endregion
    }
}
