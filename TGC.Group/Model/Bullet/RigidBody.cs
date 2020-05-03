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
        public List<RigidBody> listRigidBody = new List<RigidBody>();
        public bool isTerrain = false;
        #endregion

        #region Constructor
        public RigidBody() { }
        #endregion

        #region Metodos
        public void Initializer(Terrain terrain, CameraFPS camera, Shark shark, Ship ship, List<TgcMesh> meshes)
        {
            listRigidBody.Add(new TerrainRigidBody(terrain));
            listRigidBody.Add(new CharacterRigidBody(camera));
            listRigidBody.Add(new SharkRigidBody(shark));
            listRigidBody.Add(new OutdoorShipRigidBody(ship));
            listRigidBody.Add(new IndoorShipRigidBody(ship));
            meshes.ForEach(mesh => listRigidBody.Add(new CommonRigidBody(mesh)));

            listRigidBody.ForEach(rigidBody => rigidBody.Init());
        }

        public virtual void Init() { }
        public virtual void Teleport() { }
        public virtual void Update(TgcD3dInput input) { }

        public virtual void Render()
        {
            listRigidBody.ForEach(rigidBody => rigidBody.Render());
        }

        public virtual void Dispose()
        {
            listRigidBody.ForEach(rigidBody => rigidBody.Dispose());
        }

        public void setGravity(BTRigidBody rigidBody, float gravity)
        {
            rigidBody.Gravity = new Vector3(0, -gravity, 0);
        }

        #endregion
    }
}
