using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Core.Terrain;
using TGC.Group.Utils;

namespace TGC.Group.Model.Terrains
{
    class InsideRoom
    {
        protected string FILE_NAME;
        private string MediaDir;
        private string ShadersDir;

        private TgcScene Mesh;

        public InsideRoom(string mediaDir, string shadersDir)
        {
            FILE_NAME = "InsideRoom-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public void LoadRoom()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME, MediaDir + "\\");
            Mesh.Meshes.ForEach(paredes => {
                paredes.Scale = new TGCVector3(10.5f, 10.5f, 10.5f);
                paredes.Position = new TGCVector3(350, -2500, -45);
            });
        }

        public virtual void Render()
        {
            Mesh.RenderAll();
        }

        public virtual void Dispose()
        {
            Mesh.DisposeAll();
        }
    }
}
