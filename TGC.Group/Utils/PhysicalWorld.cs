using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Terrains;

namespace TGC.Group.Utils
{
    class PhysicalWorld
    {
        #region Atributos
        private TGCVector3 director = new TGCVector3(1, 0, 0);
        private Vector3 gravityZero = Vector3.Zero;
        private DiscreteDynamicsWorld dynamicsWorld;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;
        private RigidBody cameraRBInsideShipPosition;
        private RigidBody cameraRBOutsideShipPosition;
        private CameraFPS Camera;
        public BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;

        public float Gravity { get; set; }
        #endregion

        #region Constructor
        public PhysicalWorld(CameraFPS camera, CustomVertex.PositionTextured[] vertex)
        {
            Camera = camera;
            initDynamicsWorld();
            createRigidBodyFromTerrain(vertex);
            createRigidBodyCamera();
        }
        #endregion

        private void initDynamicsWorld()
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);
            dynamicsWorld.Gravity = gravityZero;
        }
        
        private void createRigidBodyFromTerrain(CustomVertex.PositionTextured[] vertex)
        {
            var meshRigidBody = rigidBodyFactory.CreateSurfaceFromHeighMap(vertex);
            dynamicsWorld.AddRigidBody(meshRigidBody);
        }
        
        private void createRigidBodyCamera()
        {
            var insidePosition = Camera.getShipInsidePosition();
            var outsidePosition = Camera.getShipOutsidePosition();

            cameraRBInsideShipPosition = rigidBodyFactory.CreateBall(30f, 0.75f, insidePosition);                
            dynamicsWorld.AddRigidBody(cameraRBInsideShipPosition);

            cameraRBOutsideShipPosition = rigidBodyFactory.CreateBall(30f, 0.75f, outsidePosition);            
            dynamicsWorld.AddRigidBody(cameraRBOutsideShipPosition);
        }

        public void addNewRigidBody(TgcMesh mesh)
        {
            var rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(mesh);
            dynamicsWorld.AddRigidBody(rigidBody);
        }

        public void changeGravityInY(RigidBody rigidBody, float gravity)
        {
            Gravity = -gravity;
            rigidBody.Gravity = new Vector3(0, Gravity, 0);
        }

        public void Update(TgcD3dInput input, float elapsedTime, float timeBetweenFrames)
        {
            dynamicsWorld.StepSimulation(elapsedTime, 10, timeBetweenFrames);

            var strength = 1.50f;
            var angle = 5;
            
            if (Camera.position.Y < 0)
            {
                cameraRBInsideShipPosition.ActivationState = ActivationState.ActiveTag;

                if (input.keyDown(Key.W))
                {
                    cameraRBInsideShipPosition.ApplyCentralImpulse(strength * director.ToBulletVector3());
                }

                if (input.keyDown(Key.S))
                {
                    cameraRBInsideShipPosition.ApplyCentralImpulse(-strength * director.ToBulletVector3());
                }

                if (input.keyDown(Key.A))
                {
                    director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.001f));
                }

                if (input.keyDown(Key.D))
                {
                    director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.001f));
                }

                if (input.keyPressed(Key.Space))
                {
                    cameraRBInsideShipPosition.ActivationState = ActivationState.ActiveTag;
                    cameraRBInsideShipPosition.ApplyCentralImpulse(TGCVector3.Up.ToBulletVector3() * 150);
                }
                Camera.position = new TGCVector3(cameraRBInsideShipPosition.CenterOfMassPosition.X,
                                                 cameraRBInsideShipPosition.CenterOfMassPosition.Y,
                                                 cameraRBInsideShipPosition.CenterOfMassPosition.Z);
            }
            else
            {
                cameraRBOutsideShipPosition.ActivationState = ActivationState.ActiveTag;
                cameraRBOutsideShipPosition.Gravity = new Vector3(0, -100, 0);
                
                if (input.keyDown(Key.W))
                {
                    cameraRBOutsideShipPosition.ApplyCentralImpulse(strength * director.ToBulletVector3());
                }

                if (input.keyDown(Key.S))
                {
                    cameraRBOutsideShipPosition.ApplyCentralImpulse(-strength * director.ToBulletVector3());
                }

                if (input.keyDown(Key.A))
                {
                    director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.001f));
                }

                if (input.keyDown(Key.D))
                {
                    director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.001f));
                }

                if (input.keyPressed(Key.Space))
                {
                    cameraRBOutsideShipPosition.ActivationState = ActivationState.ActiveTag;
                    cameraRBOutsideShipPosition.ApplyCentralImpulse(TGCVector3.Up.ToBulletVector3() * 150);
                }
                Camera.position = new TGCVector3(cameraRBOutsideShipPosition.CenterOfMassPosition.X,
                                                 cameraRBOutsideShipPosition.CenterOfMassPosition.Y,
                                                 cameraRBOutsideShipPosition.CenterOfMassPosition.Z);
            }
            
        }

        public void Dispose()
        {
            //Se hace dispose del modelo fisico.
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
            cameraRBInsideShipPosition.Dispose();
            cameraRBOutsideShipPosition.Dispose();

        }
    }
}
