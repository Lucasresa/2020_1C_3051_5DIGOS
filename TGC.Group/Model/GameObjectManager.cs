using BulletSharp;
using System.Linq.Expressions;
using TGC.Core.Input;
using TGC.Group.Model.Objects;
using TGC.Group.Utils;
using static TGC.Group.Model.Objects.Common;
using static TGC.Group.Model.Objects.Fish;

namespace TGC.Group.Model
{
    class GameObjectManager
    {
        private readonly string MediaDir, ShadersDir;
        private MeshBuilder MeshBuilder;
        private Ray Ray;

        public PhysicalWorld PhysicalWorld { get; set; }
        public TgcD3dInput Input { get; set; }
        public CameraFPS Camera { get; set; }
        public Character Character { get; set; }
        public Shark Shark { get; set; }
        public Ship Ship { get; set; }
        public Skybox Skybox { get; set; }
        public Terrain Terrain { get; set; }
        public Water Water { get; set; }
        public Fish Fish { get; set; }
        public Vegetation Vegetation { get; set; }
        public Common Common { get; set; }

        public (int ID, string name) ItemSelected { get; set; }

        public GameObjectManager(string mediaDir, string shadersDir, CameraFPS camera, TgcD3dInput input, Ray ray)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Camera = camera;
            Input = input;
            Ray = ray;
            InitializerObjects();
        }

        private void InitializerObjects()
        {
            /* Initializer object */

            Skybox = new Skybox(MediaDir, ShadersDir, Camera);
            Terrain = new Terrain(MediaDir, ShadersDir);
            Water = new Water(MediaDir, ShadersDir);
            MeshBuilder = new MeshBuilder(Terrain, Water);
            Ship = new Ship(MediaDir, ShadersDir);
            Shark = new Shark(MediaDir, ShadersDir, Skybox, Terrain, Camera);
            Character = new Character(Camera, Input);
            Fish = new Fish(MediaDir, ShadersDir, Skybox, Terrain);
            Vegetation = new Vegetation(MediaDir, ShadersDir);
            Common = new Common(MediaDir, ShadersDir);

            /* Location */

            MeshBuilder.LocateMeshesInWorld(meshes: ref Fish.ListFishes, area: Skybox.currentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Vegetation.ListAlgas, area: Skybox.currentPerimeter);            
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListCorals, area: Skybox.currentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListOres, area: Skybox.currentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListRock, area: Skybox.currentPerimeter);

            Fish.UpdateBoundingBox();
            Common.LocateBody();

            /* Add rigidBody to the world */

            PhysicalWorld = new PhysicalWorld();
            PhysicalWorld.AddBodyToTheWorld(Terrain.Body);
            PhysicalWorld.AddBodyToTheWorld(Character.Body);
            PhysicalWorld.AddBodyToTheWorld(Ship.BodyOutdoorShip);
            PhysicalWorld.AddBodyToTheWorld(Ship.BodyIndoorShip);
            PhysicalWorld.AddBodyToTheWorld(Shark.Body);            
            Common.ListCorals.ForEach(coral => PhysicalWorld.AddBodyToTheWorld(coral.Body));
            Common.ListOres.ForEach(ore => PhysicalWorld.AddBodyToTheWorld(ore.Body));
            Common.ListRock.ForEach(rock => PhysicalWorld.AddBodyToTheWorld(rock.Body));
        }

        public void Dispose()
        {
            Skybox.Dispose();
            Terrain.Dispose();
            Water.Dispose();
            Ship.Dispose();
            Shark.Dispose();
            Character.Dispose();
            Fish.Dispose();
            Vegetation.Dispose();
            Common.Dispose();
        }

        public void Render()
        {
            Skybox.Render();
            Terrain.Render();
            Water.Render();
            Ship.Render();
            Shark.Render();
            Fish.Render();
            Vegetation.Render();
            Common.Render();
        }

        public void Update(float elapsedTime, float timeBeetweenUpdate)
        {
            PhysicalWorld.dynamicsWorld.StepSimulation(elapsedTime, 10, timeBeetweenUpdate);
            Fish.Update(elapsedTime, Camera);
            Character.LooksAtTheHatch = Ray.intersectsWithObject(Ship.Plane.BoundingBox, distance: 500);
            Character.CanAtack = Ray.intersectsWithObject(Shark.Mesh.BoundingBox, distance: 150);
            Character.NearShip = Ray.intersectsWithObject(Ship.OutdoorMesh.BoundingBox, distance: 500);

            if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
            {
                DetectSelectedFishItem();
                DetectSelectedCoralItem();
                DetectSelectedOreItem();
            }
        }

        public void UpdateCharacter()
        {
            Character.Update(Skybox);
        }

        private void DetectSelectedCoralItem()
        {
            TypeCommon item;
            item = Common.ListCorals.Find(coral => { return Ray.intersectsWithObject(objectAABB: coral.mesh.BoundingBox, distance: 500); });

            if (item.mesh != null)
            {
                ItemSelected = (item.ID, item.name);
                PhysicalWorld.dynamicsWorld.RemoveRigidBody(item.Body);
                Common.ListCorals.Remove(item);
                item.mesh.Dispose();
            }
        }

        private void DetectSelectedOreItem()
        {
            TypeCommon item;
            item = Common.ListOres.Find(ore => Ray.intersectsWithObject(objectAABB: ore.mesh.BoundingBox, distance: 500));

            if (item.mesh != null)
            {
                ItemSelected = (item.ID, item.name);
                PhysicalWorld.dynamicsWorld.RemoveRigidBody(item.Body);
                Common.ListOres.Remove(item);
                item.mesh.Dispose();
            }
        }

        private void DetectSelectedFishItem()
        {
            if (!Character.HasARod) // TODO: Revisar, porque considero que no va en el personaje. Esto deberia ir en el inventario o en el crafteo.
                return;

            TypeFish item;
            item = Fish.ListFishes.Find(fish => Ray.intersectsWithObject(objectAABB: fish.mesh.BoundingBox, distance: 500));
            
            if (item.mesh != null)
            {
                ItemSelected = (item.ID, item.name);
                Fish.ListFishes.Remove(item);
                item.mesh.Dispose();
            }
        } 
    }
}

