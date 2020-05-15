using TGC.Core.SceneLoader;
using TGC.Core.BoundingVolumes;
using Microsoft.DirectX.Direct3D;

namespace TGC.Group.Model.Watercraft
{
    class Ship
    {
        #region Atributos
        private string FILE_NAME, MediaDir, ShadersDir;
        public TgcMesh OutdoorMesh, IndoorMesh;
        #endregion

        #region Constructor
        public Ship(string mediaDir, string shadersDir)
        {
            FILE_NAME = "ship-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            LoadShip();
        }
        #endregion

        #region Metodos
        private void LoadShip()
        {
            OutdoorMesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            IndoorMesh = OutdoorMesh.createMeshInstance("InsideRoom");
            OutdoorMesh.updateBoundingBox();
        }
        #endregion
    }
}
