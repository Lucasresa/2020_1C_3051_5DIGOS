using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Group.Model.Objects;
using TGC.Group.Utils;
namespace TGC.Group.Model
{
    public class GameMenu : TGCExample
    {
        private Water Water;
        private Skybox Skybox;
        private CameraFPS CameraFPS;
        private Ship Ship;

        private DrawMenu Menu;
        private GameModel Game;

        public GameMenu(string mediaDir, string shadersDir): base(mediaDir, shadersDir) => FixedTickEnable = true;

        public override void Dispose()
        {
            Water.Dispose();
            Skybox.Dispose();
            Ship.Dispose();
            Menu.Dispose();
        }

        public override void Init()
        {
            Camera = CameraFPS = new CameraFPS(Input);
            Water = new Water(MediaDir, ShadersDir, new TGCVector3(0, 3500, 0));
            Skybox = new Skybox(MediaDir, CameraFPS);
            Ship = new Ship(MediaDir);

            Menu = new DrawMenu(MediaDir, Input);
            Game = new GameModel(MediaDir, ShadersDir);
            CreateMenu();

            CameraFPS.Position = new TGCVector3(1030, 3900, 2500);
            CameraFPS.Lock = true;

            Game.Init();
        }

        private void CreateMenu()
        {
            Menu.CreateButton(text: "Play", scale: new TGCVector2(0.4f, 0.4f), position: new TGCVector2(20, 500), action: Game.Render);
        }

        public override void Render()
        {
            PreRender();
            Water.Render();
            Skybox.Render(Water.SizeWorld());
            Ship.RenderOutdoorShip();
            Menu.Render();
            PostRender();
        }

        public override void Update()
        {
            Water.Update(ElapsedTime);
            Menu.Update();
        }
    }
}