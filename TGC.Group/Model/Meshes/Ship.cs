using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Utils;

namespace TGC.Group.Model.Watercraft
{
    class Ship
    {
        protected string FILE_NAME;

        private string MediaDir;
        private string ShadersDir;

        public TgcMesh OutdoorMesh;
        public TgcMesh IndoorMesh;

        public Ship(string mediaDir, string shadersDir)
        {
            FILE_NAME = "ship-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public virtual void Render()
        {
            OutdoorMesh.UpdateMeshTransform();
            OutdoorMesh.Render();
            IndoorMesh.Render();
        }

        public virtual void Dispose()
        {
            OutdoorMesh.Dispose();
            IndoorMesh.Dispose();
        }

        public void LoadShip()
        {
            OutdoorMesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            //Mesh.Transform = TGCMatrix.RotationYawPitchRoll(FastMath.ToRad(-13), FastMath.ToRad(1), FastMath.ToRad(270)) *
            //                 TGCMatrix.Scaling(10, 10, 10) *
            //                 TGCMatrix.Translation(530, 3630, 100);
            OutdoorMesh.Scale = new TGCVector3(10, 10, 10);
            OutdoorMesh.Position = new TGCVector3(530, 3630, 100);
            OutdoorMesh.Rotation = new TGCVector3(-13, 1, 270);

            IndoorMesh = OutdoorMesh.createMeshInstance("InsideRoom");
            IndoorMesh.Position = new TGCVector3(350, -2500, -45);
            IndoorMesh.Scale = new TGCVector3(10, 10, 10);
            IndoorMesh.Rotation = new TGCVector3(0, FastMath.PI_HALF, 0);
        }
    }
}
