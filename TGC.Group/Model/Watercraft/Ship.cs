using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Watercraft
{
    class Ship
    {
        protected string FILE_NAME;

        private string MediaDir;
        private string ShadersDir;

        public TgcMesh Mesh;

        public Ship(string mediaDir, string shadersDir)
        {
            FILE_NAME = "ship-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public virtual void Render()
        {
            Mesh.Render();
        }

        public virtual void Dispose()
        {
            Mesh.Dispose();
        }

        public void LoadShip()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            Mesh.Scale = new TGCVector3(10, 10, 10);
            Mesh.Position = new TGCVector3(530, 3630, 100);
            Mesh.Rotation = new TGCVector3(-13, 1, 270);
        }
    }
}
