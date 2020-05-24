using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Group.Model.Status;
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

        private GameObjectManager ObjectManager;
        private CharacterStatus CharacterStatus;
        private SharkStatus SharkStatus;

        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            FixedTickEnable = true;
            D3DDevice.Instance.ZFarPlaneDistance = 12000f;
        }

        public override void Init()
        {
            Camera = new CameraFPS(Input);
            camera = (CameraFPS)Camera;
            ObjectManager = new GameObjectManager(MediaDir, ShadersDir, camera, Input);
            CharacterStatus = new CharacterStatus(ObjectManager.Character);
            SharkStatus = new SharkStatus(ObjectManager.Shark);
        }

        public override void Update()
        {
            ObjectManager.Update(ElapsedTime, TimeBetweenUpdates);
            CharacterStatus.Update();
            SharkStatus.Update();
        }

        public override void Render()
        {
            PreRender();
            ObjectManager.Render();
            PostRender();
        }

        public override void Dispose()
        {
            ObjectManager.Dispose();
        }
    }
}