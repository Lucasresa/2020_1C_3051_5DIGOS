
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
            LoadWeapon();
        }

        public virtual void Render()
        {
            Mesh.Render();
        }

        public virtual void Dispose()
        {
            Mesh.Dispose();
        }

        private void LoadWeapon()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            Mesh.Transform = TGCMatrix.Scaling(scale) * TGCMatrix.Translation(position);
        }
    }
}
