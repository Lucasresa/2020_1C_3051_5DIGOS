using BulletSharp.Math;
using System.Collections.Generic;
using BulletSharp;
using TGC.Core.Input;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Bullet.Bodies;
using TGC.Group.Model.Sharky;
using TGC.Group.Model.Terrains;
using TGC.Group.Model.Watercraft;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet
{
    class RigidBodyManager
    {
        #region Atributos
        private string MediaDir, ShadersDir;
        private Sky skybox;
        private List<CommonRigidBody> commonRigidBody = new List<CommonRigidBody>();
        private TerrainRigidBody terrainRigidBody;
        private CharacterRigidBody characterRigidBody;
        private SharkRigidBody sharkRigidBody;
        private OutdoorShipRigidBody outdoorShipRigidBody;
        private IndoorShipRigidBody indoorShipRigidBody;
        private DiscreteDynamicsWorld dynamicsWorld;
        #endregion

        #region PhysicalWorld
        private Vector3 gravityZero = Vector3.Zero;
        private CollisionDispatcher dispatcher;
        private DefaultCollisionConfiguration collisionConfiguration;
        private SequentialImpulseConstraintSolver constraintSolver;
        private BroadphaseInterface overlappingPairCache;
        #endregion

        #region Constructor
        public RigidBodyManager(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;

            #region Configuracion del mundo fisico
            collisionConfiguration = new DefaultCollisionConfiguration();
            dispatcher = new CollisionDispatcher(collisionConfiguration);
            GImpactCollisionAlgorithm.RegisterAlgorithm(dispatcher);
            constraintSolver = new SequentialImpulseConstraintSolver();
            overlappingPairCache = new DbvtBroadphase();
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration) {  Gravity = gravityZero };
            #endregion
        }
        #endregion

        #region Metodos

        public void Init(TgcD3dInput input, Terrain terrain, CameraFPS camera, Shark shark, Ship ship, Sky skyBox, ref List<TgcMesh> meshes)
        {
            skybox = skyBox;
            terrainRigidBody = new TerrainRigidBody(terrain);
            characterRigidBody = new CharacterRigidBody(input, camera, MediaDir, ShadersDir);
            sharkRigidBody = new SharkRigidBody(shark);
            outdoorShipRigidBody = new OutdoorShipRigidBody(ship);
            indoorShipRigidBody = new IndoorShipRigidBody(ship);

            dynamicsWorld.AddRigidBody(terrainRigidBody.body);
            dynamicsWorld.AddRigidBody(characterRigidBody.body);
            dynamicsWorld.AddRigidBody(sharkRigidBody.body);
            dynamicsWorld.AddRigidBody(outdoorShipRigidBody.body);
            dynamicsWorld.AddRigidBody(indoorShipRigidBody.body);

            addNewRigidBody(ref meshes);

            characterRigidBody.aabbShip = outdoorShipRigidBody.getAABB();
        }

        public void Render()
        {
            if (characterRigidBody.isInsideShip())
                indoorShipRigidBody.Render();
            else
            {
                terrainRigidBody.Render();

                if (skybox.inSkyBox(sharkRigidBody.body))
                    sharkRigidBody.Render();

                if (skybox.inSkyBox(outdoorShipRigidBody.body))
                    outdoorShipRigidBody.Render();

                commonRigidBody.ForEach(rigidBody =>
                {
                    if (skybox.inSkyBox(rigidBody.body))
                        rigidBody.Render();
                });
            }
            
            characterRigidBody.Render();
        }

        public void Update(TgcD3dInput input, float elapsedTime, float timeBetweenFrames)
        {
            characterRigidBody.Update(dynamicsWorld, ref commonRigidBody);

            if (!characterRigidBody.isInsideShip())
            {
                sharkRigidBody.Update(input, elapsedTime);
                UpdatePhysicalWorld(elapsedTime, timeBetweenFrames);
            }
            
        }

        private void UpdatePhysicalWorld(float elapsedTime, float timeBetweenFrames)
        {
            dynamicsWorld.StepSimulation(elapsedTime, 10, timeBetweenFrames);
            dynamicsWorld.UpdateSingleAabb(terrainRigidBody.body);
            dynamicsWorld.UpdateSingleAabb(characterRigidBody.body);
            dynamicsWorld.UpdateSingleAabb(sharkRigidBody.body);
            dynamicsWorld.UpdateSingleAabb(outdoorShipRigidBody.body);
            dynamicsWorld.UpdateSingleAabb(indoorShipRigidBody.body);
            commonRigidBody.ForEach(rigidBody => dynamicsWorld.UpdateSingleAabb(rigidBody.body));
        }

        public void Dispose()
        {
            dynamicsWorld.Dispose();
            dispatcher.Dispose();
            collisionConfiguration.Dispose();
            constraintSolver.Dispose();
            overlappingPairCache.Dispose();
            terrainRigidBody.Dispose();
            characterRigidBody.Dispose();
            sharkRigidBody.Dispose();
            outdoorShipRigidBody.Dispose();
            indoorShipRigidBody.Dispose();
            commonRigidBody.ForEach(rigidBody => rigidBody.Dispose());
        }

        public void addNewRigidBody(ref List<TgcMesh> meshes)
        {
            meshes.ForEach(mesh => commonRigidBody.Add(new CommonRigidBody(mesh)));
            meshes.RemoveRange(0, meshes.Count);
            commonRigidBody.ForEach(rigidBody => dynamicsWorld.AddRigidBody(rigidBody.body));
        }

        #endregion
    }
}
