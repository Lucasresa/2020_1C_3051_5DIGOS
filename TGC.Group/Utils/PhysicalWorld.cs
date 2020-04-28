using BulletSharp;
using BulletSharp.Math;
using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing.Text;
using System.Linq;
using TGC.Core.BulletPhysics;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Bullet;
using TGC.Group.Model.Terrains;

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

        private Dictionary<RigidBodyType, RigidBodies> rigidBodies;

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
            rigidBodies.Values.ToList().ForEach(rigidBody => rigidBody.Update(input));
        }

        public void Dispose()
        {
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
        }        

        public void addInitialRigidBodies(Dictionary<RigidBodyType, RigidBodies> bodies)
        {
            rigidBodies = bodies;
        }

        public void addNewRigidBody(RigidBodyType type, RigidBodies rigidBody)
        {
            rigidBodies.Add(type, rigidBody);
        }

        public void addAllDynamicsWorld()
        {
            rigidBodies.Values.ToList().ForEach(rigidBody => dynamicsWorld.AddRigidBody(rigidBody.RigidBody));
        }

        public void addDynamicsWorld(RigidBody rigidBody)
        {
            dynamicsWorld.AddRigidBody(rigidBody);
        }

        #endregion
    }
}