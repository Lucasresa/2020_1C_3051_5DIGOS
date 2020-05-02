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
            rigidBody.CenterOfMassTransform = Mesh.Transform.ToBulletMatrix();
            rigidBody.CollisionShape.LocalScaling = new Vector3(10, 10, 10);
        }
        
        // TODO: Podriamos llegar a renderizar el Mesh por aca.. no me pareceria tan mala idea..
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
