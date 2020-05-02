using BulletSharp;
using BulletSharp.Math;
using System.Collections.Generic;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;

namespace TGC.Group.Utils
{
    class PhysicalWorld
    {
        #region Atributos

        private Vector3 gravityZero = Vector3.Zero;
        private readonly BulletRigidBodyFactory rigidBodyFactory = BulletRigidBodyFactory.Instance;

        #region De configuracion del bullet
        private DiscreteDynamicsWorld dynamicsWorld;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;
        #endregion

        private List<Model.Bullet.RigidBody> rigidBodies;

        #endregion

        #region Constructor
        public static PhysicalWorld Instance { get; } = new PhysicalWorld();
        private PhysicalWorld()
        {
            initDynamicsWorld();
        }
        #endregion

        #region Metodos

        private void initDynamicsWorld()
        {
            #region Configuracion del mundo fisico

            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration);

            dynamicsWorld.Gravity = gravityZero;
            #endregion            
        }

        public void Update(TgcD3dInput input, float elapsedTime, float timeBetweenFrames)
        {
            dynamicsWorld.StepSimulation(elapsedTime, 10, timeBetweenFrames);
            rigidBodies.ForEach(rigidBody =>
            {
                dynamicsWorld.UpdateSingleAabb(rigidBody.rigidBody);
                rigidBody.Update(input);
            });
        }

        public void Render()
        {
            rigidBodies.ForEach(rigidBody => rigidBody.Render());
        }

        public void Dispose()
        {
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
        }

        public void addInitialRigidBodies(List<Model.Bullet.RigidBody> bodies)
        {
            rigidBodies = bodies;
        }

        public void addRigidBodyToWorld(Model.Bullet.RigidBody rigidBody)
        {
            rigidBodies.Add(rigidBody);
            dynamicsWorld.AddRigidBody(rigidBody.rigidBody);
        }

        public void addAllDynamicsWorld()
        {
            rigidBodies.ForEach(rigidBody => dynamicsWorld.AddRigidBody(rigidBody.rigidBody));
        }

        #endregion
    }
}