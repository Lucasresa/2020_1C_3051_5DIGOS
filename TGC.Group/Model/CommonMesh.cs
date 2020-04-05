using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    abstract class CommonMesh
    {
        protected string MeshName;
        protected readonly string MediaDir;
        protected TGCVector3 Center;
        private TGCVector3 scale = new TGCVector3(10, 10, 10);

        public TgcMesh Mesh { get; set; }

        public CommonMesh(string mediaDir, TGCVector3 center, string meshName)
        {
            MediaDir = mediaDir;
            Center = center;
            MeshName = meshName;
            LoadMesh();
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

        private void LoadMesh()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + MeshName + "-TgcScene.xml").Meshes[0];
        }
    }

}
