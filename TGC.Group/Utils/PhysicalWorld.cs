using BulletSharp;
using BulletSharp.Math;
using System.Collections.Generic;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using btRigidBody = TGC.Group.Model.Bullet.RigidBody;


namespace TGC.Group.Utils
{
    class PhysicalWorld
    {
        #region Atributos

        private Vector3 gravityZero = Vector3.Zero;

        #region De configuracion del bullet
        private DiscreteDynamicsWorld dynamicsWorld;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;
        #endregion

        private List<btRigidBody> rigidBodies;

        #endregion

        #region Constructor
        public static PhysicalWorld Instance { get; } = new PhysicalWorld();
        private PhysicalWorld()
        {
            #region Configuracion del mundo fisico

            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration)
            {
                Gravity = gravityZero
            };

            #endregion   
        }
        #endregion

        #region Metodos

        public void Update(TgcD3dInput input, float elapsedTime, float timeBetweenFrames)
        {
            dynamicsWorld.StepSimulation(elapsedTime, 10, timeBetweenFrames);
            rigidBodies.ForEach(rigidBody =>
            {
                dynamicsWorld.UpdateSingleAabb(rigidBody.body);
                rigidBody.Update(input, elapsedTime);
            });
        }

        public void Dispose()
        {
            rigidBodies.ForEach(rigidBody => rigidBody.Dispose());
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
        }

        public void addInitialRigidBodies(List<btRigidBody> bodies)
        {
            rigidBodies = bodies;
            rigidBodies.ForEach(rigidBody => dynamicsWorld.AddRigidBody(rigidBody.body));
        }

        public void addRigidBodyToWorld(btRigidBody rigidBody)
        {
            rigidBodies.Add(rigidBody);
            dynamicsWorld.AddRigidBody(rigidBody.body);
        }

        #endregion
    }
}