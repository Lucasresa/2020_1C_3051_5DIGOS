
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Meshes
{
    class Weapon
    {
        protected string FILE_NAME;

        private string MediaDir;
        private string ShadersDir;

        public TgcMesh Mesh;

        private TGCVector3 scale = new TGCVector3(3, 3, 3);
        private TGCVector3 position = new TGCVector3(1300, 3505, 20);

        public Weapon(string mediaDir, string shadersDir)
        {
            FILE_NAME = "EspadaDoble-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public virtual void Render()
        {
            Mesh.UpdateMeshTransform();
            Mesh.Render();
        }

        public virtual void Dispose()
        {
            Mesh.Dispose();
        }

        public void Init()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            Mesh.Position = position;
            Mesh.Scale = scale;
        }

    }
}
