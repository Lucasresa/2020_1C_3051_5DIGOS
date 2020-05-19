
using System;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;

namespace TGC.Group.Model.Meshes
{
    class Weapon
    {
        protected string FILE_NAME;

        private string MediaDir;
        private string ShadersDir;

        public TgcMesh Mesh;
        
        private TGCVector3 scale = new TGCVector3(2, 2, 2);
        private TGCVector3 position = new TGCVector3(1300, 3505, 20);
        private float RotationYOffset = FastMath.QUARTER_PI / 1.5f;
        private float RotationXOffset = FastMath.PI_HALF;
        private float AtackRotation = 0f;

        public bool Atacking { get; private set; } = false;
        public bool AtackLocked { get; private set; } = false;

        public Weapon(string mediaDir, string shadersDir)
        {
            FILE_NAME = "EspadaDoble-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Init();
        }

        private void Init()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
        }

        public void Update(CameraFPS camera, TGCVector3 cameraDirection, float elapsedTime)
        {
            float RotationStep = FastMath.PI * 2.5f * elapsedTime;
            CalculateRotationByAtack(RotationStep);
            AtackLocked = !(AtackRotation <= 0);

            var localSideAxis = TGCVector3.Cross(TGCVector3.Up, cameraDirection);
            localSideAxis.Normalize();
            var forwardOffset = cameraDirection * 43;
            var sideOffset = localSideAxis * 15;
            TGCVector3 newPosition = camera.position + forwardOffset + sideOffset;
            
            Mesh.Transform = TGCMatrix.Scaling(scale) * 
                             TGCMatrix.RotationYawPitchRoll(camera.latitude - RotationYOffset, camera.longitude + RotationXOffset - AtackRotation, 0) *
                             TGCMatrix.Translation(newPosition);
        }

        public void Render()
        {
            Mesh.Render();
        }

        public void Dispose()
        {
            Mesh.Dispose();
        }

        public void ActivateAtackMove()
        {
            if (Atacking || AtackLocked)
                return;
            Atacking = true;
            AtackLocked = true;
        }

        private void CalculateRotationByAtack(float rotationStep)
        {
            if (Atacking)
            {
                Atacking = AtackRotation <= FastMath.PI_HALF;
                AtackRotation += rotationStep;
            }
            else if (AtackRotation > 0)
                AtackRotation += -rotationStep * 0.5f;
        }

    }
}
