using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Sharky
{
    class Shark
    {
        #region Atributos
        protected string FILE_NAME;
        private string MediaDir;
        private string ShadersDir;

        public TgcMesh Mesh;
        #endregion

        #region Constructor
        public Shark(string mediaDir, string shadersDir)
        {
            FILE_NAME = "shark-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }
        #endregion

        #region Metodos
        public void LoadShark()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
        }

        public virtual void Render()
        {
            Mesh.Render();
        }

        public virtual void Dispose()
        {
            Mesh.Dispose();
        }
        #endregion
    }
}
