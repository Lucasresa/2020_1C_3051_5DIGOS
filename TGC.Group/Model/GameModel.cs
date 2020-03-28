using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Example;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model
{
    public class GameModel : TgcExample
    {
        public GameModel(string mediaDir, string shadersDir) : base(mediaDir, shadersDir)
        {
            Category = Game.Default.Category;
            Name = Game.Default.Name;
            Description = Game.Default.Description;
        }

        public override void Init()
        {
            var d3dDevice = D3DDevice.Instance.Device;


        }

        public override void Update()
        {
            PreUpdate();
            PostUpdate();
        }

        
        public override void Render()
        { 
            PreRender();

            //Dibuja un texto por pantalla
            DrawText.drawText("Primera prueba", 0, 20, Color.OrangeRed);
         
            PostRender();
        }

        public override void Dispose()
        {

        }
    }
}