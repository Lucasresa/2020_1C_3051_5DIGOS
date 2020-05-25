using BulletSharp;
using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Linq;
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
        private readonly Ray Ray;

        public PhysicalWorld PhysicalWorld { get; set; }
        public TgcD3dInput Input { get; set; }
        public CameraFPS Camera { get; set; }
        public Character Character { get; set; }
        public Shark Shark { get; set; }
        public Ship Ship { get; set; }
        public Skybox Skybox { get; set; }
        public Terrain Terrain { get; set; }
        public Water Water { get; set; }
        public List<Fish> Fishes { get; set; }
        public Vegetation Vegetation { get; set; }
        public Common Common { get; set; }

        public string ItemSelected { get; set; }
        public bool NearObjectForSelect { get; set; }

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
            Vegetation = new Vegetation(MediaDir, ShadersDir);
            Common = new Common(MediaDir, ShadersDir);
            Fishes = Common.ListFishes.Select(mesh => new Fish(ShadersDir, Skybox, Terrain, mesh)).ToList();

            /* Location */

            MeshBuilder.LocateMeshesInWorld(meshes: ref Vegetation.ListAlgas, area: Skybox.currentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListCorals, area: Skybox.currentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListOres, area: Skybox.currentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListRock, area: Skybox.currentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListFishes, area: Skybox.currentPerimeter);

            Common.LocateObjects();

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
            Fishes.ForEach(fish => fish.Dispose());
            Vegetation.Dispose();
            Common.Dispose();
        }

        public void Render()
        {
            if (Character.IsInsideShip)
                Ship.RenderIndoorShip();
            else
            {
                Ship.RenderOutdoorShip();
                Skybox.Render(Terrain.SizeWorld());
                Terrain.Render();
                Water.Render();
                Shark.Render();
                Fishes.ForEach(fish => fish.Render());
                Vegetation.Render();
                Common.Render();
            }
        }

        public void Update(float elapsedTime, float timeBeetweenUpdate)
        {
            PhysicalWorld.dynamicsWorld.StepSimulation(elapsedTime, maxSubSteps: 10, timeBeetweenUpdate);
            Fishes.ForEach(fish => fish.Update(elapsedTime, Camera));
            Character.LooksAtTheHatch = Ray.intersectsWithObject(objectAABB: Ship.Plane.BoundingBox, distance: 500);
            Character.CanAtack = Ray.intersectsWithObject(objectAABB: Shark.Mesh.BoundingBox, distance: 150);
            Character.NearShip = Ray.intersectsWithObject(objectAABB: Ship.OutdoorMesh.BoundingBox, distance: 500);
            DetectSelectedItem();
        }

        public void UpdateCharacter()
        {
            Character.Update(Skybox);
        }

        private void DetectSelectedItem()
        {
            bool NearCoralForSelect = false;
            bool NearOreForSelect = false;
            bool NearFishForSelect = false;
                        
            TypeCommon Coral = Common.ListCorals.Find(coral => NearCoralForSelect = Ray.intersectsWithObject(objectAABB: coral.mesh.BoundingBox, distance: 500));
            TypeCommon Ore = Common.ListOres.Find(ore => NearOreForSelect = Ray.intersectsWithObject(objectAABB: ore.mesh.BoundingBox, distance: 500));

            if (Character.CanFish && Coral.mesh is null && Ore.mesh is null)
            {
                Fish itemFish = Fishes.Find(fish => NearFishForSelect = Ray.intersectsWithObject(objectAABB: fish.BoundingBox, distance: 500));
                if (NearFishForSelect) SelectItem(itemFish);
            }
            
            NearObjectForSelect = NearCoralForSelect || NearOreForSelect || NearFishForSelect;

            if (NearCoralForSelect)
                SelectItem(Coral);
            else if (NearOreForSelect)
                SelectItem(Ore);
        }

        private void SelectItem(TypeCommon item)
        {
            if (item.mesh != null && Input.keyPressed(Key.E))
            {
                ItemSelected = item.name;
                PhysicalWorld.RemoveBodyToTheWorld(item.Body);
                if (item.name.ToUpper().Contains("CORAL"))
                    Common.ListCorals.Remove(item);
                else
                    Common.ListOres.Remove(item);
            }
        }

        private void SelectItem(Fish item)
        {
            if (Input.keyPressed(Key.E))
            {
                ItemSelected = item.Mesh.name;
                Fishes.Remove(item);
                Common.ListFishes.Remove(item.Mesh);
            }
        }
    }
}