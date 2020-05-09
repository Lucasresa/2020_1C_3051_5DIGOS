using BulletSharp;
using BulletSharp.Math;
using System;
using System.Collections.Generic;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class CommonRigidBody
    {
        #region Atributos
        public Vector3 Scale = new Vector3(10, 10, 10);
        private BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;

        public TgcMesh Mesh;
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
            body.CollisionShape.LocalScaling = Scale;
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

        #endregion
    }
}
