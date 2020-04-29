using BulletSharp;
using BulletSharp.Math;
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

namespace TGC.Group.Model.Bullet.Bodies
{
    class ShipRigidBody : RigidBodies
    {
        public Ship Ship;

        TGCVector3 pos = new TGCVector3(350, -2500, -45);

        Shark tiburoncito = new Shark(Game.Default.MediaDirectory, Game.Default.ShadersDirectory);

        public ShipRigidBody(RigidBodyType type, Ship ship)
        {
            Type = type;
            Ship = ship;

        }
        
        private TGCSphere sphereMesh;

        public override void Init()
        {            
            //RigidBody = rigidBodyFactory.CreateBall(500f, 1f, new TGCVector3(530, 3620, 100));
            //var textureDragonBall = TgcTexture.createTexture(D3DDevice.Instance.Device, Game.Default.MediaDirectory + @"Textures\blue.jpg");
            //sphereMesh = new TGCSphere(1, textureDragonBall, TGCVector3.Empty);   
            //sphereMesh.updateValues();

            switch (Type)
            {
                case RigidBodyType.insideShip:
                    //RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Ship.InsideMesh);
                    break;
                case RigidBodyType.outsideShip:
                    
                    tiburoncito.LoadShark();
                    tiburoncito.Mesh.Position = Ship.Mesh.Position;
                    RigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(tiburoncito.Mesh);
                    break;
            }
            RigidBody.Translate(Ship.Mesh.Position.ToBulletVector3());
        }

        public override void Render()
        {
            //sphereMesh.Transform = TGCMatrix.Scaling(300, 300, 300) * new TGCMatrix(RigidBody.InterpolationWorldTransform);
            //sphereMesh.Render();
            //Ship.Mesh.Transform = TGCMatrix.Scaling(1, 1, 1) * new TGCMatrix(RigidBody.InterpolationWorldTransform);
            //Ship.Render();
            tiburoncito.Mesh.Transform = TGCMatrix.Scaling(1, 1, 1) * new TGCMatrix(RigidBody.InterpolationWorldTransform);
            tiburoncito.Render();

        }

        public override void Update(TgcD3dInput input)
        {
            RigidBody.ActivationState = ActivationState.ActiveTag;
        }

        public override void Dispose()
        {
            RigidBody.Dispose();
            Ship.Dispose();
        }
    }
}
