using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Watercraft
{
    class Ship
    {
        protected string FILE_NAME;

        private string MediaDir;
        private string ShadersDir;

        public TgcMesh OutdoorMesh;
        public TgcMesh IndoorMesh;

        public Ship(string mediaDir, string shadersDir)
        {
            FILE_NAME = "ship-TgcScene.xml";
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
        }

        public virtual void Render()
        {
            OutdoorMesh.Render();
            IndoorMesh.Render();
        }

        public virtual void Dispose()
        {
            OutdoorMesh.Dispose();
            IndoorMesh.Dispose();
        }

        public void LoadShip()
        {
            OutdoorMesh = new TgcSceneLoader().loadSceneFromFile(MediaDir + FILE_NAME).Meshes[0];
            OutdoorMesh.Position = new TGCVector3(530, 3630, 100);

            IndoorMesh = OutdoorMesh.createMeshInstance("InsideRoom");
            IndoorMesh.Position = new TGCVector3(350, -2500, -45);
        }
    }
}
