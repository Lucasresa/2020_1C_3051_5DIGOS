using BulletSharp;
using BulletSharp.Math;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Terrains;
using Microsoft.DirectX.DirectInput;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    class FishMesh
    {
        private struct Constants
        {
            public static TGCVector2 FishHeight = new TGCVector2(700, 2000);
            public static TGCVector3 Scale = new TGCVector3(10, 10, 10);
            public static float MaxYRotation = FastMath.PI - FastMath.QUARTER_PI;
            public static float MaxAxisRotation = FastMath.QUARTER_PI;
        }

        private TGCVector3 director;
        private float acumulatedXRotation;
        private float acumulatedYRotation;
        private bool activateMove;
        private TGCMatrix TotalRotation;
        
        private Sky skybox;
        private Terrain terrain;
        
        public TgcMesh Mesh;
        public FishMesh(TgcMesh mesh, Sky sky, Terrain terrain)
        {
            Mesh = mesh;
            skybox = sky;
            this.terrain = terrain;
            director = new TGCVector3(0, 0, 1);
        }

        public void Init()
        {
            acumulatedXRotation = 0;
            acumulatedYRotation = 0;
            activateMove = false;
            TotalRotation = TGCMatrix.Identity;
            Mesh.BoundingBox.scaleTranslate(Mesh.Position, Constants.Scale);
        }

        public void Update(TgcD3dInput input, float elapsedTime, TGCVector3 cameraPosition)
        {

            if (IsNearFromPlayer(cameraPosition))
                ChangeFishWay();
            else if (activateMove)
                PerformNormalMove(elapsedTime, 500, GetFishHeadPosition());

            if (input.keyPressed(Key.U)) activateMove = !activateMove;
        }

        public void Render()
        {
            Mesh.Render();
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        private void PerformNormalMove(float elapsedTime, float speed, TGCVector3 headPosition)
        {
            float XRotation = 0f, YRotation = 0f;
            var meshPosition = GetMeshPosition();
            terrain.world.interpoledHeight(headPosition.X, headPosition.Z, out float floorHeight);
            var distanceToFloor = meshPosition.Y - floorHeight;
            var XRotationStep = FastMath.PI * 0.1f * elapsedTime;

            if (distanceToFloor < Constants.FishHeight.X - 40 && acumulatedXRotation < Constants.MaxAxisRotation)
                XRotation = XRotationStep;
            else if (IsNumberBetweenInterval(distanceToFloor, Constants.FishHeight) && acumulatedXRotation > 0.0012)
                XRotation = -XRotationStep;
            if (distanceToFloor > Constants.FishHeight.Y + 40 && acumulatedXRotation > -Constants.MaxAxisRotation)
                XRotation = -XRotationStep;
            else if (IsNumberBetweenInterval(distanceToFloor, Constants.FishHeight) && acumulatedXRotation < -0.0012)
                XRotation = XRotationStep;

            if (!skybox.inPerimeterSkyBox(meshPosition.X, meshPosition.Z) && 
                                                        FastMath.Abs(acumulatedYRotation) < Constants.MaxYRotation)
                YRotation = RotationYAxis(elapsedTime);
            else
                acumulatedYRotation = 0;

            acumulatedXRotation += XRotation;
            acumulatedYRotation += YRotation;
            TGCMatrix rotation = TGCMatrix.Identity;
            if (XRotation != 0 || FastMath.Abs(acumulatedXRotation) > 0.0012)
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
            Mesh.Transform = TGCMatrix.Scaling(Constants.Scale) * TotalRotation * traslation;
            Mesh.BoundingBox.transform(Mesh.Transform);
        }


        private bool IsNearFromPlayer(TGCVector3 cameraPosition)
        {
            var distanceToPlayer = (cameraPosition - GetFishHeadPosition()).Length();
            return distanceToPlayer < 1000;
        }
        private void ChangeFishWay()
        {
            director.TransformCoordinate(TGCMatrix.RotationY(FastMath.PI_HALF));
            TotalRotation *= TGCMatrix.RotationY(FastMath.PI_HALF);
            Mesh.Transform = TGCMatrix.Scaling(Constants.Scale) * TotalRotation * TGCMatrix.Translation(GetMeshPosition());
        }

        private float RotationYAxis(float elapsedTime)
        {
            var bodyToSkyboxCenterVector = TGCVector3.Normalize(skybox.getSkyboxCenter() - GetMeshPosition());
            var actualDirector = -1 * director;
            var normalVector = TGCVector3.Cross(actualDirector, bodyToSkyboxCenterVector);
            var rotationStep = FastMath.PI * 0.3f * elapsedTime;
            return normalVector.Y > 0 ? rotationStep : -rotationStep;
        }

        private bool IsNumberBetweenInterval(float number, TGCVector2 interval)
        {
            return number > interval.X && number < interval.Y;
        }

        private TGCVector3 GetMeshPosition()
        {
            var transform = Mesh.Transform.ToBulletMatrix();
            return new TGCVector3(transform.Row4.X, transform.Row4.Y, transform.Row4.Z);
        }

        private TGCVector3 GetFishHeadPosition()
        {
            var distanceToHead = Mesh.BoundingBox.calculateBoxRadius();
            return GetMeshPosition() + director * -distanceToHead;
        }

    }
}