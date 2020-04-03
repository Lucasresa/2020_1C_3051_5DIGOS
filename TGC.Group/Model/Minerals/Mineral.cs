using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Minerals
{
    abstract class Mineral
    {
        protected string FILE_NAME;
        private readonly string MediaDir;
        protected TGCVector3 Center;

        public TgcMesh Mesh { get; set; }

        public Mineral(string mediaDir, TGCVector3? center)
        {
            MediaDir = mediaDir;
            Center = center ?? new TGCVector3(0f, 3550f, 0f);
        }

        public virtual void Init()
        {
            Mesh.Position = Center;
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
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];            
        }
    }
}
