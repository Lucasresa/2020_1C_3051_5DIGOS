using BulletSharp;
using BulletSharp.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.BulletPhysics;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;

namespace TGC.Group.Model.Objects
{
    class Weapon
    {
        protected string FILE_NAME;

        private string MediaDir;
        private string ShadersDir;

        public TgcMesh Mesh;

        private TGCVector3 scale = new TGCVector3(2, 2, 2);
        private TGCVector3 position = new TGCVector3(1300, 3505, 20);
        private float MaxForwardRotation = FastMath.PI_HALF;
        private float MaxSideRotation = FastMath.QUARTER_PI / 1.3f;
        private float RotationXOffset = FastMath.PI_HALF;
        private float AttackForwardRotation = 0f;
        private float AttackSideRotation = 0f;
        public bool Attacking { get; private set; } = false;
        public bool AttackLocked { get; private set; } = false;

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

        public void Update(CameraFPS camera, float elapsedTime)
        {
            float RotationStep = FastMath.PI * 2.5f * elapsedTime;
            CalculateRotationByAtack(RotationStep);
            AttackLocked = !(AttackForwardRotation <= 0) && !(AttackSideRotation <= 0);

            var localSideAxis = TGCVector3.Cross(TGCVector3.Up, camera.Direction);
            localSideAxis.Normalize();
            var forwardOffset = camera.Direction * 48;
            var sideOffset = localSideAxis * 15;
            TGCVector3 newPosition = camera.Position + forwardOffset + sideOffset;

            Mesh.Transform = TGCMatrix.Scaling(scale) *
                             TGCMatrix.RotationYawPitchRoll(camera.Latitude - AttackSideRotation, camera.Longitude + RotationXOffset - AttackForwardRotation, 0) *
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
            if (Attacking || AttackLocked)
                return;
            Attacking = true;
            AttackLocked = true;
        }

        private void CalculateRotationByAtack(float rotationStep)
        {
            if (Attacking)
            {
                Attacking = AttackForwardRotation <= MaxForwardRotation || AttackSideRotation <= MaxSideRotation;
                AttackForwardRotation += AttackForwardRotation <= MaxForwardRotation ? rotationStep : 0;
                AttackSideRotation += AttackSideRotation <= MaxSideRotation ? rotationStep / 2 : 0;
            }
            else if (AttackForwardRotation > 0 || AttackSideRotation > 0)
            {
                AttackForwardRotation += AttackForwardRotation > 0 ? -rotationStep * 0.5f : 0;
                AttackSideRotation += AttackSideRotation > 0 ? -rotationStep * 0.25f : 0;
            }
        }

    }
}
