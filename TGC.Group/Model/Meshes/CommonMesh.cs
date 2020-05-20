using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    class CommonMesh
    {
        #region Atributos
        private string MeshName, MediaDir, ShadersDir;
        public TgcMesh Mesh;
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
