using System.Collections.Generic;
using System.Linq;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;

namespace TGC.Group.Model.Objects
{
    class Fish
    {
        public struct TypeFish
        {
            public int ID;
            public string name;
            public TgcMesh mesh;
        }

        private struct Constants
        {
            public static string NAME_FISH_NORMAL = "normalFish";
            public static string NAME_FISH_YELLOW = "yellowFish";
            public static int QUANTITY_FISH_NORMAL = 20;
            public static int QUANTITY_FISH_YELLOW = 20;
            public static TGCVector2 FishHeight = new TGCVector2(700, 2000);
            public static TGCVector3 Scale = new TGCVector3(10, 10, 10);
            public static float MaxYRotation = FastMath.PI - FastMath.QUARTER_PI;
            public static float MaxAxisRotation = FastMath.QUARTER_PI;
            public static float ScapeFromPlayerCooldown = 3;
        }

        private string MediaDir, ShadersDir;
        private TGCVector3 director;
        private float acumulatedXRotation  = 0;
        private float acumulatedYRotation = 0;
        private TGCMatrix TotalRotation;
        private float time;
        private readonly Skybox Skybox;
        private readonly Terrain Terrain;
        private TypeFish normalFish;
        private TypeFish yellowFish;
        private TgcMesh currentMesh;

        public bool ActivateMove { get; set; }
        public List<TypeFish> ListFishes = new List<TypeFish>();

        public Fish(string mediaDir, string shadersDir, Skybox skybox, Terrain terrain)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            director = new TGCVector3(0, 0, 1);
            Skybox = skybox;
            Terrain = terrain;
            Init();
        }

        public void Dispose()
        {
            ListFishes.ForEach(fish => fish.mesh.Dispose());
        }

        private void Init()
        {
            time = Constants.ScapeFromPlayerCooldown;
            TotalRotation = TGCMatrix.Identity;
            InitializerFishes();
            GenerateDuplicates(normalFish, ref ListFishes);
            GenerateDuplicates(yellowFish, ref ListFishes);
        }

        private void InitializerFishes()
        {
            normalFish.name = Constants.NAME_FISH_NORMAL;
            normalFish.ID = Constants.QUANTITY_FISH_NORMAL;
            LoadInitial(ref normalFish);

            yellowFish.name = Constants.NAME_FISH_YELLOW;
            yellowFish.ID = Constants.QUANTITY_FISH_YELLOW;
            LoadInitial(ref yellowFish);
        }

        private void LoadInitial(ref TypeFish fish)
        {
            fish.mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + fish.name + "-TgcScene.xml").Meshes[0];
            fish.mesh.Name = fish.name;
        }

        public void GenerateDuplicates(TypeFish fish, ref List<TypeFish> fishes)
        {
            foreach (int index in Enumerable.Range(1, fish.ID))
            {
                TypeFish newFish = new TypeFish
                {
                    ID = index,
                    name = fish.name + "_" + index
                };
                newFish.mesh = fish.mesh.createMeshInstance(newFish.name);
                newFish.mesh.Transform = TGCMatrix.Scaling(Constants.Scale);                
                fishes.Add(newFish);
            }
        }

        public void Update(float elapsedTime, CameraFPS camera)
        {
            ListFishes.ForEach(fish =>
            {
                currentMesh = fish.mesh;
                if (IsNearFromPlayer(camera.Position) && time <= 0)
                    ChangeFishWay();
                else if (ActivateMove)
                    PerformNormalMove(elapsedTime, speed: 500, GetFishHeadPosition());
            });
        }

        public void UpdateBoundingBox()
        {
            ListFishes.ForEach(fish => fish.mesh.BoundingBox.scaleTranslate(fish.mesh.Position, Constants.Scale));
        }

        public void Render()
        {
            ListFishes.ForEach(fish => fish.mesh.Render());
        }
               
        private void PerformNormalMove(float elapsedTime, float speed, TGCVector3 headPosition)
        {
            time -= elapsedTime;
            
            float XRotation = 0f, YRotation = 0f;
            var meshPosition = GetMeshPosition();

            Terrain.world.interpoledHeight(headPosition.X, headPosition.Z, out float floorHeight);
            var distanceToFloor = FastUtils.Distance(meshPosition.Y, floorHeight);

            var XRotationStep = FastMath.PI * 0.1f * elapsedTime;
            var YRotationStep = FastMath.PI * 0.3f * elapsedTime;

            if (FastUtils.LessThan(distanceToFloor, Constants.FishHeight.X - 40) && FastUtils.LessThan(acumulatedXRotation, Constants.MaxAxisRotation))
                XRotation = XRotationStep;
            else if (FastUtils.IsNumberBetweenInterval(distanceToFloor, Constants.FishHeight) && FastUtils.GreaterThan(acumulatedXRotation, 0.0012f))
                XRotation = -XRotationStep;
            if (FastUtils.GreaterThan(distanceToFloor, Constants.FishHeight.Y + 40) && FastUtils.GreaterThan(acumulatedXRotation, -Constants.MaxAxisRotation))
                XRotation = -XRotationStep;
            else if (FastUtils.IsNumberBetweenInterval(distanceToFloor, Constants.FishHeight) && FastUtils.LessThan(acumulatedXRotation, -0.0012f))
                XRotation = XRotationStep;

            if (!Skybox.InPerimeterSkyBox(meshPosition.X, meshPosition.Z) && FastUtils.LessThan(FastMath.Abs(acumulatedYRotation), Constants.MaxYRotation))
                YRotation = YRotationStep * RotationYSign();
            else
                acumulatedYRotation = 0;

            acumulatedXRotation += XRotation;
            acumulatedYRotation += YRotation;

            TGCMatrix rotation = TGCMatrix.Identity;

            if (XRotation != 0 || FastUtils.GreaterThan(FastMath.Abs(acumulatedXRotation), 0.0012f))
            {
                var rotationAxis = TGCVector3.Cross(TGCVector3.Up, director);
                director.TransformCoordinate(TGCMatrix.RotationAxis(rotationAxis, XRotation));
                rotation = TGCMatrix.RotationAxis(rotationAxis, XRotation);
                speed /= 1.5f;
            }
            else if (YRotation != 0)
            {
                director.TransformCoordinate(TGCMatrix.RotationY(YRotation));
                rotation = TGCMatrix.RotationY(YRotation);
            }

            TotalRotation *= rotation;
            TGCMatrix traslation = TGCMatrix.Translation(meshPosition + director * -speed * elapsedTime);
            currentMesh.Transform = TGCMatrix.Scaling(Constants.Scale) * TotalRotation * traslation;
            currentMesh.BoundingBox.transform(currentMesh.Transform);
        }

        private bool IsNearFromPlayer(TGCVector3 cameraPosition)
        {
            return FastUtils.IsDistanceBetweenVectorsLessThan(distance: 1000, vectorA: cameraPosition, vectorB: GetFishHeadPosition());
        }

        private void ChangeFishWay()
        {
            TGCMatrix Rotation = TGCMatrix.RotationY(-RotationYSign() * FastMath.PI_HALF);
            director.TransformCoordinate(Rotation);
            TotalRotation *= Rotation;
            currentMesh.Transform = TGCMatrix.Scaling(Constants.Scale) * TotalRotation * TGCMatrix.Translation(GetMeshPosition());
            time = Constants.ScapeFromPlayerCooldown;
        }

        private float RotationYSign()
        {
            var bodyToSkyboxCenterVector = TGCVector3.Normalize(Skybox.GetSkyboxCenter() - GetMeshPosition());
            var actualDirector = -1 * director;
            var normalVector = TGCVector3.Cross(actualDirector, bodyToSkyboxCenterVector);
            return normalVector.Y > 0 ? 1 : -1;
        }

        private TGCVector3 GetMeshPosition()
        {
            var transform = currentMesh.Transform.ToBulletMatrix();
            return new TGCVector3(transform.Row4.X, transform.Row4.Y, transform.Row4.Z);
        }

        private TGCVector3 GetFishHeadPosition()
        {
            var distanceToHead = currentMesh.BoundingBox.calculateBoxRadius();
            return GetMeshPosition() + director * -distanceToHead;
        }
    }
}