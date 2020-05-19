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
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Bullet
{
    class RigidBodyManager
    {
        #region Atributos
        private string MediaDir, ShadersDir;
        private Sky skybox;
        private InventoryManagement inventory;
        private List<FishMesh> fishes = new List<FishMesh>();
        private List<CommonRigidBody> commonRigidBody = new List<CommonRigidBody>();
        private TerrainRigidBody terrainRigidBody;
        private CharacterRigidBody characterRigidBody;
        private SharkRigidBody sharkRigidBody;
        private OutdoorShipRigidBody outdoorShipRigidBody;
        private IndoorShipRigidBody indoorShipRigidBody;
        private DiscreteDynamicsWorld dynamicsWorld;
        private GameEventsManager gameEventsManager;
        private CameraFPS Camera;
        private Crafting crafting;
        private TgcD3dInput Input;
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

        public void Init(TgcD3dInput input, Terrain terrain, CameraFPS camera, Shark shark, Ship ship, Sky skyBox, 
            ref List<TgcMesh> meshes, List<FishMesh> fishes)
        {
            skybox = skyBox;
            Camera = camera;
            Input = input;
            inventory = new InventoryManagement(MediaDir, ShadersDir, Input);
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
            this.fishes = fishes;
            gameEventsManager = new GameEventsManager(sharkRigidBody, characterRigidBody);
            characterRigidBody.aabbShip = outdoorShipRigidBody.getAABB();
        }
        public void Update(TgcD3dInput input, float elapsedTime, float timeBetweenFrames)
        {
            dynamicsWorld.StepSimulation(elapsedTime, 10, timeBetweenFrames);
            characterRigidBody.Update(elapsedTime, sharkRigidBody, skybox);
            inventory.Update(input, dynamicsWorld, ref commonRigidBody, ref fishes, Camera.lockCam);
            crafting.Update(input);
            characterRigidBody.status.Update(crafting.hasADivingHelmet);

            gameEventsManager.Update(elapsedTime, fishes);

            sharkRigidBody.Update(input, elapsedTime, characterRigidBody.status);
            fishes.ForEach(fish => fish.Update(input, elapsedTime, Camera.position));

            if (Input.keyPressed(Key.I))
            {
                Camera.lockCam = !Camera.lockCam;
                inventory.changePointer();
            }
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

                if (gameEventsManager.SharkIsAttacking)
                    sharkRigidBody.Render();

                if (skybox.Contains(outdoorShipRigidBody.body))
                    outdoorShipRigidBody.Render();

                commonRigidBody.ForEach(rigidBody =>
                {
                    if (skybox.Contains(rigidBody.body))
                        rigidBody.Render();
                });
                fishes.ForEach(fish =>  fish.Render());
            }
            #endregion
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
            fishes.ForEach(fish => fish.Dispose());
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
