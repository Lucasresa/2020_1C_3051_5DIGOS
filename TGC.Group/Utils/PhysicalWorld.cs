﻿using BulletSharp;
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
        private DiscreteDynamicsWorld dynamicsWorld;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;
        private RigidBody rigidBody;
        private TGCVector3 director = new TGCVector3(1, 0, 0);
        private CameraFPS Camera;
        public BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;
        public float Gravity { get; set; }
        #endregion

        #region Constructor
        public PhysicalWorld(CameraFPS camera, Terrain terrain, float gravity)
        {
            Camera = camera;
            createDynamicsWorld(gravity);
            createRigidBodyFromTerrain(terrain);
            createRigidCamera();
        }
        #endregion

        private void createDynamicsWorld(float gravity)
        {
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);
            changeGravityInY(gravity);
        }
        
        private void createRigidBodyFromTerrain(Terrain terrain)
        {
            var meshRigidBody = rigidBodyFactory.CreateSurfaceFromHeighMap(terrain.world.getVertices());
            dynamicsWorld.AddRigidBody(meshRigidBody);
        }
        
        private void createRigidCamera()
        {
            rigidBody = rigidBodyFactory.CreateBall(30f, 0.75f, Camera.position);
            dynamicsWorld.AddRigidBody(rigidBody);
        }

        public void addNewRigidBody(TgcMesh mesh)
        {
            var rigidBody = rigidBodyFactory.CreateRigidBodyFromTgcMesh(mesh);
            dynamicsWorld.AddRigidBody(rigidBody);
        }

        public void changeGravityInY(float gravity)
        {
            Gravity = -gravity;                
            dynamicsWorld.Gravity = new TGCVector3(0, Gravity, 0).ToBulletVector3();
        }

        public void Update(TgcD3dInput input, float elapsedTime, float timeBetweenFrames)
        {
            dynamicsWorld.StepSimulation(elapsedTime, 10, timeBetweenFrames);
                                   
            //var strength = 1.50f;
            //var angle = 5;
            //
            //rigidBody.ActivationState = ActivationState.ActiveTag;
            //
            //if (input.keyDown(Key.W))
            //{
            //    rigidBody.ApplyCentralImpulse(strength * director.ToBulletVector3());
            //}
            //
            //if (input.keyDown(Key.S))
            //{
            //    rigidBody.ApplyCentralImpulse(-strength * director.ToBulletVector3());
            //}
            //
            //if (input.keyDown(Key.A))
            //{
            //    director.TransformCoordinate(TGCMatrix.RotationY(-angle * 0.001f));
            //}
            //
            //if (input.keyDown(Key.D))
            //{
            //    director.TransformCoordinate(TGCMatrix.RotationY(angle * 0.001f));
            //}
            //
            //if (input.keyPressed(Key.Space))
            //{
            //    rigidBody.ActivationState = ActivationState.ActiveTag;
            //    rigidBody.ApplyCentralImpulse(TGCVector3.Up.ToBulletVector3() * 150);
            //}
            
            Camera.position = new TGCVector3(rigidBody.CenterOfMassPosition.X,
                                             rigidBody.CenterOfMassPosition.Y,
                                             rigidBody.CenterOfMassPosition.Z);
            
        }

        public void Dispose()
        {
            //Se hace dispose del modelo fisico.
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
        }
    }
}
