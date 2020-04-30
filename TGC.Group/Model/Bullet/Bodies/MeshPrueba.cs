using BulletSharp;
using BulletSharp.Math;
using BulletSharp.SoftBody;
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
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    class MeshPrueba : RigidBodies
    {
        private Shark Sharky = new Shark(Game.Default.MediaDirectory, Game.Default.ShadersDirectory);
        private CameraFPS Camera;

        TGCVector3 posicion = new TGCVector3(530, 3630, 100);


        public MeshPrueba(Utils.CameraFPS camera)
        {
            Camera = camera;
            Sharky.LoadShark();
        }
        
        public override void Init()
        {
            RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Sharky.Mesh);
            RigidBody.Translate(posicion.ToBulletVector3());
            RigidBody.CollisionShape.LocalScaling = new Vector3(10, 10, 10);
        }

        public override void Render()
        {
            Sharky.Mesh.Position = new TGCVector3(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y + 0, RigidBody.CenterOfMassPosition.Z);
            Sharky.Mesh.Transform = TGCMatrix.Scaling(10, 10, 10) * TGCMatrix.Translation(RigidBody.CenterOfMassPosition.X, RigidBody.CenterOfMassPosition.Y, RigidBody.CenterOfMassPosition.Z);
            Sharky.Mesh.Render();
        }

        #region Metodos
        public override void Update(TgcD3dInput input)
        {
            RigidBody.ActivationState = ActivationState.ActiveTag;
        }

        public override void Dispose()
        {
            RigidBody.Dispose();
            Sharky.Dispose();
        }
        #endregion

    }
}
