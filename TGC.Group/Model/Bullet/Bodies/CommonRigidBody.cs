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
            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            body.CenterOfMassTransform = TGCMatrix.Translation(Mesh.Position).ToBulletMatrix();
            body.CollisionShape.LocalScaling = Scale;
        }
        
        public override void Render()
        {
            Mesh.Render();
        }

        public override void Dispose()
        {
            body.Dispose();
            Mesh.Dispose();
        }

        public override TgcMesh getMesh()
        {
            return Mesh;
        }

        #endregion
    }
}
