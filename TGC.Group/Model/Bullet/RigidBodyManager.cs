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
using TGC.Group.Model.Inventory;
using TGC.Group.Model.Craft;
using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Bullet
{
    class RigidBodyManager
    {
        #region Atributos
        private string MediaDir, ShadersDir;
        private Sky skybox;
        private InventoryManagement inventory;
        private List<CommonRigidBody> commonRigidBody = new List<CommonRigidBody>();
        private TerrainRigidBody terrainRigidBody;
        private CharacterRigidBody characterRigidBody;
        private SharkRigidBody sharkRigidBody;
        private OutdoorShipRigidBody outdoorShipRigidBody;
        private IndoorShipRigidBody indoorShipRigidBody;
        private DiscreteDynamicsWorld dynamicsWorld;
        private CameraFPS Camera;
        private Crafting crafting;
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
            dynamicsWorld = new DiscreteDynamicsWorld(dispatcher, overlappingPairCache, constraintSolver, collisionConfiguration) { Gravity = gravityZero };
            #endregion
        }
        #endregion

        #region Metodos

        public void Init(TgcD3dInput input, Terrain terrain, CameraFPS camera, Shark shark, Ship ship, Sky skyBox, ref List<TgcMesh> meshes)
        {
            skybox = skyBox;
            Camera = camera;
            inventory = new InventoryManagement(MediaDir, ShadersDir, input);
            crafting = new Crafting(MediaDir, ShadersDir, inventory.items);

            #region Agregar rigidos al mundo fisico
            terrainRigidBody = new TerrainRigidBody(terrain);
            characterRigidBody = new CharacterRigidBody(input, camera, MediaDir, ShadersDir);
            sharkRigidBody = new SharkRigidBody(shark, skybox, terrain, camera);
            outdoorShipRigidBody = new OutdoorShipRigidBody(ship);
            indoorShipRigidBody = new IndoorShipRigidBody(ship);

            dynamicsWorld.AddRigidBody(terrainRigidBody.body);
            dynamicsWorld.AddRigidBody(characterRigidBody.body);
            dynamicsWorld.AddRigidBody(sharkRigidBody.body);
            dynamicsWorld.AddRigidBody(outdoorShipRigidBody.body);
            dynamicsWorld.AddRigidBody(indoorShipRigidBody.body);

            addNewRigidBody(ref meshes);
            #endregion

            characterRigidBody.aabbShip = outdoorShipRigidBody.getAABB();
        }

        public void Render()
        {
            inventory.Render();

            #region Renderizar deacuerdo a la posicion del personaje
            characterRigidBody.Render();

            if (characterRigidBody.isInsideShip())
                indoorShipRigidBody.Render();
            else
            {
                terrainRigidBody.Render();

                if (skybox.Contains(sharkRigidBody.body))
                    sharkRigidBody.Render();

                if (skybox.Contains(outdoorShipRigidBody.body))
                    outdoorShipRigidBody.Render();

                commonRigidBody.ForEach(rigidBody =>
                {
                    if (skybox.Contains(rigidBody.body))
                        rigidBody.Render();
                });
            }
            #endregion
        }

        public void Update(TgcD3dInput input, float elapsedTime, float timeBetweenFrames)
        {
            dynamicsWorld.StepSimulation(elapsedTime, 10, timeBetweenFrames);
            characterRigidBody.Update(elapsedTime, sharkRigidBody);

            if (!characterRigidBody.isInsideShip())
                sharkRigidBody.Update(input, elapsedTime, characterRigidBody.status);

            inventory.Update(input, dynamicsWorld, ref commonRigidBody, Camera.lockCam);

            if (Camera.Position.Y > 0)
                return;

            if (input.keyDown(Key.M))
                crafting.craftWeapon();

            if (input.keyDown(Key.N))
                crafting.craftRod();

            if (input.keyDown(Key.B))
                crafting.craftDivingHelmet();
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
            inventory.Dispose();
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
