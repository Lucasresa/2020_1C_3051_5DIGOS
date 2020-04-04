using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Sharky
{
    class Shark
    {
        protected string FILE_NAME;

        private string MediaDir;
        private string ShadersDir;

        public TgcMesh Mesh;

        private TGCVector3 scale = new TGCVector3(3, 3, 3);
        private TGCVector3 position = new TGCVector3(-2185, 420 , -525);


        public Shark(string mediaDir, string shadersDir)
        {
            FILE_NAME = "shark-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public virtual void Init()
        {
           
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

        public void loadMesh()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            Mesh.Position = position;
            Mesh.Scale = scale;
        }
    }
}
