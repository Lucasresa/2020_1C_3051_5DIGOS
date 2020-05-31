using Microsoft.DirectX.DirectInput;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TGC.Core.Fog;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Shaders;
using TGC.Core.Textures;
using TGC.Group.Model.Objects;
using TGC.Group.Utils;
using static TGC.Group.Model.Objects.Common;
using Effect = Microsoft.DirectX.Direct3D.Effect;

namespace TGC.Group.Model
{
    class GameObjectManager
    {
        private readonly string MediaDir, ShadersDir;
        private MeshBuilder MeshBuilder;
        private readonly Ray Ray;
        private Effect FogShader;

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
        public bool ShowInfoItemCollect { get; set; }

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
            FogShader = TGCShaders.Instance.LoadEffect(ShadersDir + "SmartTerrain.fx");

            FogShader.SetValue("ColorFog", Color.MidnightBlue.ToArgb());
            FogShader.SetValue("StartFogDistance", 2000);
            FogShader.SetValue("EndFogDistance", 8000);
            FogShader.SetValue("Density", 0.0025f);

            /* Initializer object */

            Skybox = new Skybox(MediaDir, Camera);
            Terrain = new Terrain(MediaDir, ShadersDir);
            Water = new Water(MediaDir, ShadersDir);
            MeshBuilder = new MeshBuilder(Terrain, Water);
            Ship = new Ship(MediaDir);
            Shark = new Shark(MediaDir, Skybox, Terrain, Camera);
            Character = new Character(Camera, Input);

            Vegetation = new Vegetation(MediaDir);
            Common = new Common(MediaDir);
            Fishes = Common.ListFishes.Select(mesh => new Fish(MediaDir, Skybox, Terrain, mesh)).ToList();

            /* Location */

            MeshBuilder.LocateMeshesInWorld(meshes: ref Vegetation.ListAlgas, area: Skybox.CurrentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListCorals, area: Skybox.CurrentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListOres, area: Skybox.CurrentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListRock, area: Skybox.CurrentPerimeter);
            MeshBuilder.LocateMeshesInWorld(meshes: ref Common.ListFishes, area: Skybox.CurrentPerimeter);

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

            Common.SetShader(FogShader, "Fog");
            Ship.SetShader(FogShader, "Fog");
            Vegetation.SetShader(FogShader, "Fog");
            Skybox.SetShader(FogShader, "Fog");
            Shark.SetShader(FogShader, "Fog");
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
                FogShader.SetValue("CameraPos", TGCVector3.TGCVector3ToFloat4Array(Camera.Position));
                Ship.RenderOutdoorShip();
                Skybox.Render(Terrain.SizeWorld());
                Terrain.Render();
                Shark.Render();
                Fishes.ForEach(fish => fish.Render());
                Vegetation.Render();
                Common.Render();
                Water.Render();
            }
        }

        public void Update(float elapsedTime, float timeBeetweenUpdate)
        {
            PhysicalWorld.dynamicsWorld.StepSimulation(elapsedTime, maxSubSteps: 10, timeBeetweenUpdate);

            Fishes.ForEach(fish => fish.Update(elapsedTime, Camera));
            Skybox.Update();
            Shark.Update(elapsedTime);
            Water.Update(elapsedTime);
            Terrain.Update(elapsedTime);

            Character.LooksAtTheHatch = Ray.intersectsWithObject(objectAABB: Ship.Plane.BoundingBox, distance: 500);
            Character.CanAttack = Ray.intersectsWithObject(objectAABB: Shark.Mesh.BoundingBox, distance: 150);
            Character.NearShip = Ray.intersectsWithObject(objectAABB: Ship.OutdoorMesh.BoundingBox, distance: 500);
            Character.IsNearSkybox = Skybox.IsNearSkybox;
            DetectSelectedItem();
        }

        public void UpdateCharacter() => Character.Update();

        private void DetectSelectedItem()
        {
            bool NearCoralForSelect = false;
            bool NearOreForSelect = false;
            bool NearFishForSelect = false;
                        
            TypeCommon Coral = Common.ListCorals.Find(coral => NearCoralForSelect = Ray.intersectsWithObject(objectAABB: coral.Mesh.BoundingBox, distance: 500));
            TypeCommon Ore = Common.ListOres.Find(ore => NearOreForSelect = Ray.intersectsWithObject(objectAABB: ore.Mesh.BoundingBox, distance: 500));

            if (Character.CanFish && Coral.Mesh is null && Ore.Mesh is null)
            {
                Fish itemFish = Fishes.Find(fish => NearFishForSelect = Ray.intersectsWithObject(objectAABB: fish.BoundingBox, distance: 500));
                if (NearFishForSelect) SelectItem(itemFish);
            }
            
            NearObjectForSelect = NearCoralForSelect || NearOreForSelect || NearFishForSelect;

            if (NearCoralForSelect) SelectItem(Coral);
            else if (NearOreForSelect) SelectItem(Ore);
        }

        private void SelectItem(TypeCommon item)
        {
            if (item.Mesh != null && Input.keyPressed(Key.E))
            {
                ShowInfoItemCollect = true;
                ItemSelected = item.Name;
                PhysicalWorld.RemoveBodyToTheWorld(item.Body);
                if (item.Name.Contains("CORAL")) Common.ListCorals.Remove(item);
                else Common.ListOres.Remove(item);
            }
        }

        private void SelectItem(Fish item)
        {
            if (Input.keyPressed(Key.E))
            {
                ItemSelected = item.Mesh.Name;
                Fishes.Remove(item);
                Common.ListFishes.Remove(item.Mesh);
            }
        }
    }
}