using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class CommonMesh
    {
        #region Atributos
        protected string MeshName;
        protected readonly string MediaDir;
        public TgcMesh Mesh { get; set; }
        #endregion

        #region Constructor
        public CommonMesh(string mediaDir, string meshName)
        {
            MediaDir = mediaDir;
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

        #endregion
    }
}