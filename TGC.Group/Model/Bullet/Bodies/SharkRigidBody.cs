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
        private float SHARK_HEIGHT = 300; 

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
        private float MaxAxisRotation = FastMath.PI;
        private float acumulatedYRotation = 0;
        private float acumulatedAxisRotation = 0;
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
            var YRotation = FastMath.PI * 0.7f * elapsedTime;
            var axisStepRotation = FastMath.PI_HALF * 0.7f * elapsedTime;
            var speed = 400;

            /***
             * Hacer que el tiburon se mueva y que detecte si tiene obstaculos delante de el
             * Hacer que solo se mueva dentro del skybox para que este dentro del rango del jugador (personaje)
             * Hacer que el tiburon persiga al jugador y busque colisionar con este
             * Una vez que haga la colision el tiburon debera volver a su movimiento normal de "asechar" por un tiempo y luego volver a atacar
             * 
             * 
             */
            
            if(activateMove)
            {
                var headOffsetPosition = director * -1000; 
                terrain.world.interpoledHeight(body.CenterOfMassPosition.X + headOffsetPosition.X, body.CenterOfMassPosition.Z + headOffsetPosition.Z, out float Y);

                /*if (body.CenterOfMassPosition.Y - Y < SHARK_HEIGHT - 50)
                {
                    acumulatedAxisRotation += axisStepRotation;
                    if (MaxAxisRotation - acumulatedAxisRotation > 0)
                    {
                        TGCVector3 rotationAxis = TGCVector3.Cross(TGCVector3.Up, director);
                        director.TransformCoordinate(TGCMatrix.RotationAxis(rotationAxis, axisStepRotation));
                        mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationAxis(rotationAxis, axisStepRotation) * new TGCMatrix(body.InterpolationWorldTransform);
                        body.WorldTransform = mesh.Transform.ToBulletMatrix();
                    }
                }
                else if (body.CenterOfMassPosition.Y - Y >= SHARK_HEIGHT && acumulatedAxisRotation != 0) 
                {
                    var rotationAxis = TGCVector3.Cross(TGCVector3.Up, director);
                    director.TransformCoordinate(TGCMatrix.RotationAxis(rotationAxis, -acumulatedAxisRotation));
                    mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationAxis(rotationAxis, -acumulatedAxisRotation) * new TGCMatrix(body.InterpolationWorldTransform);
                    body.WorldTransform = mesh.Transform.ToBulletMatrix();
                    acumulatedAxisRotation = 0;

                }
                */
                if (!skybox.Contains(this))
                {
                    acumulatedYRotation += YRotation;
                    if (MaxYRotation - acumulatedYRotation > 0)
                    {
                        director.TransformCoordinate(TGCMatrix.RotationY(YRotation));
                        mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(YRotation) * new TGCMatrix(body.InterpolationWorldTransform);
                        body.WorldTransform = mesh.Transform.ToBulletMatrix();
                    } else
                        acumulatedYRotation = 0;
                }

                body.ActivationState = ActivationState.ActiveTag;
                body.LinearVelocity = director.ToBulletVector3() * -speed;
                ray.Origin = new TGCVector3(body.CenterOfMassPosition);
            }

            if (input.keyPressed(Key.T)) activateMove = !activateMove;

            if (input.keyDown(Key.UpArrow))
            {
                //Activa el comportamiento de la simulacion fisica para la capsula
                body.ActivationState = ActivationState.ActiveTag;
                body.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                body.ApplyCentralImpulse(-100 * director.ToBulletVector3());
            }

            if (input.keyDown(Key.DownArrow))
            {
                //Activa el comportamiento de la simulacion fisica para la capsula
                body.ActivationState = ActivationState.ActiveTag;
                body.AngularVelocity = TGCVector3.Empty.ToBulletVector3();
                body.ApplyCentralImpulse(100 * director.ToBulletVector3());
            }

            if (input.keyDown(Key.LeftArrow))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(-YRotation * 0.01f));
                mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(-YRotation * 0.01f) * new TGCMatrix(body.InterpolationWorldTransform);
                body.WorldTransform = mesh.Transform.ToBulletMatrix();
            }

            if (input.keyDown(Key.RightArrow))
            {
                director.TransformCoordinate(TGCMatrix.RotationY(YRotation * 0.01f));
                mesh.Transform = TGCMatrix.Translation(TGCVector3.Empty) * TGCMatrix.RotationY(YRotation * 0.01f) * new TGCMatrix(body.InterpolationWorldTransform);
                body.WorldTransform = mesh.Transform.ToBulletMatrix();
            }

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
