using BulletSharp;
using BulletSharp.Math;
using System;
using System.Collections.Generic;
using TGC.Core.BoundingVolumes;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class CommonRigidBody
    {
        #region Atributos
        struct Constants
        {
            public static TGCVector3 Scale = new TGCVector3(10, 10, 10);
        }
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;

        public readonly TgcMesh Mesh;
        public RigidBody body;
        #endregion

        #region Constructor
        public CommonRigidBody(TgcMesh mesh)
        {
            Mesh = mesh;
            Init();
        }
        #endregion

        #region Metodos
        private void Init()
        {
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CenterOfMassTransform = TGCMatrix.Translation(Mesh.Position).ToBulletMatrix();
            body.CollisionShape.LocalScaling = Constants.Scale.ToBulletVector3();
            Mesh.BoundingBox.scaleTranslate(Mesh.Position, Constants.Scale);
        }
        
        public void Render()
        {
            Mesh.Render();
        }

        public void Dispose()
        {
            body.Dispose();
            Mesh.Dispose();
        }
        public TgcBoundingAxisAlignBox getAABB()
        {
            return Mesh.BoundingBox;
        }

        public string getName()
        {
            return Mesh.Name;
        }
        #endregion
    }
}
