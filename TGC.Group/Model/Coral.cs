using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class Coral
    {

        private string MediaDir;
        private TGCVector3 Center;

        public TgcMesh Mesh { get; set; }

        public Coral(string mediaDir)
        {
            MediaDir = mediaDir;
            Center = new TGCVector3(0f, 3550f, 0f);
        }

        public Coral(string mediaDir, TGCVector3 center)
        {
            MediaDir = mediaDir;
            Center = center;
        }


        public void Init()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + "coral-TgcScene.xml").Meshes[0];
            Mesh.Position = Center;
        }

        public void Render()
        {
            Mesh.Render();
        }

        public void Update()
        {

        }

    }


}
