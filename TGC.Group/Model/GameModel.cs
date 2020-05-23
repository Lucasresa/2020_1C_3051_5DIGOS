using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    public class GameModel : TGCExample
    {
        private CameraFPS camera;

        public struct Perimeter
        {
            public float xMin, xMax, zMin, zMax;

            public Perimeter(float xMin, float xMax, float zMin, float zMax)
            {
                this.xMin = xMin;
                this.xMax = xMax;
                this.zMin = zMin;
                this.zMax = zMax;
            }
        }

        private GameObjectManager objectManager;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FixedTickEnable = true;
            D3DDevice.Instance.ZFarPlaneDistance = 12000f;
        }

        public override void Init()
        {
            Camera = new CameraFPS(Input);
            camera = (CameraFPS)Camera;
            objectManager = new GameObjectManager(MediaDir, ShadersDir, camera, Input);
        }

        public override void Update()
        {
            objectManager.Update(ElapsedTime, TimeBetweenUpdates);
        }

        public override void Render()
        {
            PreRender();
            objectManager.Render();
            PostRender();
        }

        public override void Dispose()
        {
            objectManager.Dispose();
        }
    }
}