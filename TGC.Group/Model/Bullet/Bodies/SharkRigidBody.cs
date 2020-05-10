using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.DirectInput;
using System.Linq;
using TGC.Core.Collision;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;

namespace TGC.Group.Model.Bullet.Bodies
{
    class SharkRigidBody : RigidBody
    {
        #region Atributos
        private float SHARK_HEIGHT = 600; 

        private TGCVector3 scale = new TGCVector3(5, 5, 5);
        private TGCVector3 position = new TGCVector3(-2885, 1420, -525);
        private TGCVector3 director = new TGCVector3(0, 0, 1);
        private TGCVector3 prevPosition;
        private TGCMatrix rotation = TGCMatrix.Identity;
        private TgcRay ray;
        private Sky skybox;
        private Terrain terrain;
        private bool activateMove = false;
        private float MaxYRotation = FastMath.PI;
        private float MaxAxisRotation = FastMath.PI_HALF;
        private float acumulatedYRotation = 0;
        private float acumulatedXRotation = 0;
        #endregion

        #region Constructor

        public SharkRigidBody(Shark shark, Sky sky, Terrain terrain)
        {
            this.mesh = shark.Mesh;
            ray = new TgcRay();
            prevPosition = position;

            ray.Origin = position;
            ray.Direction = director;
            skybox = sky;
            this.terrain = terrain;
        }

        #endregion

        #region Metodos

        public override void Init()
        {
            Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
            //            body = rigidBodyFactory.CreateRigidBodyFromTgcMesh(Mesh);
            //            body.SetMassProps(1, new Vector3(1, 1, 1));
            body = rigidBodyFactory.CreateBox(new TGCVector3(88, 77, 280) * 2, 1000, position, 0, 0, 0, 0, false);
//            body.CollisionShape.LocalScaling = scale.ToBulletVector3();
            body.CenterOfMassTransform = TGCMatrix.Translation(position).ToBulletMatrix();
        }

        public override void Update(TgcD3dInput input, float elapsedTime)
        {
            var speed = 1200;

            /***
             * Hacer que el tiburon se mueva y que detecte si tiene obstaculos delante de el
             * Hacer que solo se mueva dentro del skybox para que este dentro del rango del jugador (personaje)
             * Hacer que el tiburon persiga al jugador y busque colisionar con este
             * Una vez que haga la colision el tiburon debera volver a su movimiento normal de "asechar" por un tiempo y luego volver a atacar
             * 
             * 
             */
            var XRotation = 0f;
            var YRotation = 0f;

            if (activateMove)
            {
                var headOffsetPosition = director * -1000; 
                terrain.world.interpoledHeight(body.CenterOfMassPosition.X + headOffsetPosition.X, body.CenterOfMassPosition.Z + headOffsetPosition.Z, out float Y);
            
                if (body.CenterOfMassPosition.Y - Y < SHARK_HEIGHT - 50 && acumulatedXRotation < MaxAxisRotation)
                    XRotation = FastMath.PI * 0.1f * elapsedTime;                   
                else if (body.CenterOfMassPosition.Y - Y >= SHARK_HEIGHT + 50 && acumulatedXRotation > 0) 
                    XRotation = -FastMath.PI * 0.1f * elapsedTime;
                if (!skybox.Contains(this) && MaxYRotation - acumulatedYRotation > 0)
                    YRotation = FastMath.PI * 0.5f * elapsedTime;
                else 
                    acumulatedYRotation = 0;

                acumulatedXRotation += XRotation;
                acumulatedYRotation += YRotation;

                body.ActivationState = ActivationState.ActiveTag;
                body.LinearVelocity = director.ToBulletVector3() * -speed;
                ray.Origin = new TGCVector3(body.CenterOfMassPosition);

                director.TransformCoordinate(TGCMatrix.RotationY(YRotation));
                var rotationAxis = TGCVector3.Cross(TGCVector3.Up, director);
                director.TransformCoordinate(TGCMatrix.RotationAxis(rotationAxis, XRotation));

                TGCQuaternion rotationX = TGCQuaternion.RotationAxis(new TGCVector3(1.0f, 0.0f, 0.0f), XRotation);
                TGCQuaternion rotationY = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 1.0f, 0.0f), YRotation);
                TGCQuaternion rotationZ = TGCQuaternion.RotationAxis(new TGCVector3(0.0f, 0.0f, 1.0f), 0);
                TGCQuaternion rotation = rotationX * rotationY * rotationZ;
                if (XRotation != 0 || YRotation != 0)
                {
                    mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) *
                                     TGCMatrix.RotationTGCQuaternion(rotation) * 
                                     new TGCMatrix(body.InterpolationWorldTransform);
                    body.WorldTransform = mesh.Transform.ToBulletMatrix();
                }
            }

            if (input.keyPressed(Key.T)) activateMove = !activateMove;

            if (input.keyDown(Key.P))
            {
                body.LinearVelocity = Vector3.Zero;
                body.CenterOfMassTransform = TGCMatrix.Translation(position).ToBulletMatrix();
                director = new TGCVector3(0, 0, 1);
                activateMove = false;
            }
        }

        public override void Render()
        {
            mesh.Transform = TGCMatrix.Scaling(scale) * new TGCMatrix(body.InterpolationWorldTransform);
            mesh.Render();
        }

        public override void Dispose()
        {
            body.Dispose();
            mesh.Dispose();
        }



        #endregion
    }
}
