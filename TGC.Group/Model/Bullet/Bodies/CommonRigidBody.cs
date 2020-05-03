using BulletSharp.Math;
using System;
using System.Collections.Generic;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Watercraft;

namespace TGC.Group.Model.Bullet.Bodies
{
    class CommonRigidBody : RigidBody
    {
        #region Atributos
        public TgcMesh Mesh;
        public Vector3 Scale = new Vector3(10, 10, 10);
        #endregion

        #region Constructor
        public CommonRigidBody(TgcMesh mesh)
        {
            Mesh = mesh;
        }
        #endregion

        #region Metodos
        public override void Init()
        {
            rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            rigidBody.CenterOfMassTransform = TGCMatrix.Translation(Mesh.Position).ToBulletMatrix();
            rigidBody.CollisionShape.LocalScaling = Scale;
        }
        
        public override void Render()
        {
            Mesh.Render();
        }

        public override void Dispose()
        {
            rigidBody.Dispose();
            Mesh.Dispose();
        }
        #endregion
    }
}
