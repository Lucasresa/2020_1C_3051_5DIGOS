using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class CommonMesh
    {
        #region Atributos
        protected string MeshName;
        protected readonly string MediaDir;
        protected TGCVector3 position;
        private TGCVector3 scale = new TGCVector3(1, 1, 1);

        public TgcMesh Mesh { get; set; }
        #endregion

        #region Constructor
        public CommonMesh(string mediaDir, TGCVector3 center, string meshName)
        {
            MediaDir = mediaDir;
            position = center;
            MeshName = meshName;
            LoadMesh();
        }
        #endregion

        #region Metodos
        private void LoadMesh()
        {
            Mesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + MeshName + "-TgcScene.xml").Meshes[0];
            Mesh.Name = MeshName;
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