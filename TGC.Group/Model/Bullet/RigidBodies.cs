using BulletSharp;
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
using TGC.Group.Model.Terrains;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet
{
    class RigidBodies
    {
        #region Atributos
        protected BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        public Dictionary<RigidBodyType, RigidBodies> rigidBodies = new Dictionary<RigidBodyType, RigidBodies>();

        protected RigidBodyType Type;
        protected TgcMesh Mesh;
        protected TGCVector3 Position;
        protected TGCVector3 Gravity;
        public RigidBody RigidBody;

        #endregion

        #region Constructor
        public RigidBodies() { }
        #endregion

        #region Metodos
        public void Initializer(Terrain terrain, CameraFPS camera)
        {
            rigidBodies.Add(RigidBodyType.terrain, new TerrainRigidBody(terrain));
            rigidBodies.Add(RigidBodyType.outsideCharacter, new CharacterRigidBody(RigidBodyType.outsideCharacter, camera));
            //rigidBodies.Add(RigidBodyType.insideCharacter, new CharacterRigidBody());
            //rigidBodies.Add(RigidBodyType.shark, new SharkRigidBody());
            //rigidBodies.Add(RigidBodyType.insideShip, new ShipRigidBody());
            //rigidBodies.Add(RigidBodyType.outsideShip, new ShipRigidBody());

            rigidBodies.Values.ToList().ForEach(rigidBody => rigidBody.Init());
        }

        public virtual void Init() { }
        public virtual void Update(TgcD3dInput input) { }
        public virtual void Render() { }

        public virtual void Dispose()
        {
            rigidBodies.Values.ToList().ForEach(rigidBody => rigidBody.Dispose());
        }        

        //public void setGravity(RigidBody rigidBody, float gravity)
        //{
        //    rigidBody.Gravity = new Vector3(0, -gravity, 0);
        //}
        
        //rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
        //
        //rigidBody = rigidBodyFactory.CreateBall(30f, 0.75f, position);
        
        #endregion
    }
}
