using Microsoft.DirectX.Direct3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Mathematica;
using TGC.Group.Model.Objects;
using TGC.Group.Utils;

namespace TGC.Group.Model
{
    public class GameMenu : TGCExample
    {
        readonly Water Water;
        readonly Skybox Skybox;
        readonly CameraFPS CameraFPS;
        readonly DXGui Gui;

        public GameMenu(string mediaDir, string shadersDir): base(mediaDir, shadersDir)
        {
            FixedTickEnable = true;
            Gui = new DXGui();
            CameraFPS = new CameraFPS(Input);
            Camera = CameraFPS;
            Water = new Water(mediaDir, shadersDir, TGCVector3.Empty);
            Skybox = new Skybox(mediaDir, CameraFPS);
        }

        public override void Dispose()
        {
            Gui.Dispose();
            Water.Dispose();
            Skybox.Dispose();
        }

        public override void Init()
        {
            Gui.Create(MediaDir);
            // menu principal
            Gui.InitDialog(trapezoidal: false);
            int W = D3DDevice.Instance.Width;
            int H = D3DDevice.Instance.Height;
            int x0 = 70;
            int y0 = H/2;
            int dy = 80;
            int dy2 = dy;
            int dx = 200;
            Gui.InsertMenuItem(1, "     Play", "open.png", x0, y0, MediaDir, dx, dy);
            Gui.InsertMenuItem(2, "   Settings", "navegar.png", x0, y0 += dy2, MediaDir, dx, dy);
            Gui.InsertMenuItem(3, "   Controls", "navegar.png", x0, y0 += dy2, MediaDir, dx, dy);
            Gui.InsertMenuItem(4, "     Exit", "salir.png", x0, y0 += dy2, MediaDir, dx, dy);

            CameraFPS.Position = new TGCVector3(0, 200, 0);
            CameraFPS.Lock = false;
            //Camera.SetCamera(new TGCVector3(0, 3000, 0), new TGCVector3(0, 0, -1));
        }

        public void gui_render(float elapsedTime)
        {
            // ------------------------------------------------
            GuiMessage msg = Gui.Update(elapsedTime, Input);

            // proceso el msg
            switch (msg.message)
            {
                case MessageType.WM_COMMAND:
                    switch (msg.id)
                    {
                        case 1:
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
            Gui.Render();
        }

        public override void Render()
        {
            PreRender();

            Water.Render();
            Skybox.Render(Water.SizeWorld());
            gui_render(ElapsedTime);

            PostRender();
        }

        public override void Update()
        {
            //Mati LPM 
            Water.Update(ElapsedTime);
        }
    }
}
