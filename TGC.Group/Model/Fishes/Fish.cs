using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Fishes
{
    class Fish
    {
        protected string FishName;
        private readonly string MediaDir;
        protected TGCVector3 Center;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);

        public TgcMesh Mesh { get; set; }

        public Fish(string mediaDir, TGCVector3? center, string meshName)
        {
            MediaDir = mediaDir;
            Center = center ?? new TGCVector3(0f, 3550f, 0f);
            FishName = meshName;
        }

        public virtual void Init()
        {
            Mesh.Position = Center;
            Mesh.Scale = scale;
        }

        public virtual void Render()
        {
            Mesh.Render();
        }

        public virtual void Update()
        {

        }

        public virtual void Dispose()
        {
            Mesh.Dispose();
        }

        public void LoadMesh()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FishName + "-TgcScene.xml").Meshes[0];
        }
    }
}
