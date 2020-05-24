using TGC.Core.Input;
using TGC.Group.Model.Objects;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    class GameObjectManager
    {
        private readonly string MediaDir, ShadersDir;
        private PhysicalWorld PhysicalWorld;
        private MeshBuilder MeshBuilder;

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

        public GameObjectManager(string mediaDir, string shadersDir, CameraFPS camera, TgcD3dInput input)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Camera = camera;
            Input = input;
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
            Character.Update(Skybox);
            Fish.Update(elapsedTime, Camera);
        }
    }
}
