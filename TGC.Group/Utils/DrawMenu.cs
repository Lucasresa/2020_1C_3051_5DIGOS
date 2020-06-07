using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Utils
{
    class DrawButton
    {
        public Action Action { get; set; }
        public DrawText ButtonText { get; set; }
        public DrawSprite MarkedButton { get; set; }
        public DrawSprite UnmarkedButton { get; set; }
        public TGCVector2 Position { get; set; }
        public TGCVector2 Scale { get; set; }
        public TGCVector2 Size { get; set; }

        private bool IsMarked;
        private TgcD3dInput Input;

        public DrawButton(string mediaDir, TgcD3dInput input)
        {
            Input = input;
            ButtonText = new DrawText();
            MarkedButton = new DrawSprite(mediaDir);
            UnmarkedButton = new DrawSprite(mediaDir);
        }

        public void InitializerButton(string text, TGCVector2 scale, TGCVector2 position, Action action)
        {
            Scale = scale;
            Position = position;
            Action = action;
            MarkedButton.SetImage("marked.png");
            MarkedButton.SetInitialScallingAndPosition(scale, position);
            UnmarkedButton.SetImage("unmarked.png");
            UnmarkedButton.SetInitialScallingAndPosition(scale, position);
            Size = MarkedButton.Size;
            ButtonText.SetTextAndPosition(text, position: new TGCVector2((position.X - Size.X) / 2,
                                                                        (position.Y - Size.Y) / 2));
        }

        public void Dispose()
        {
            ButtonText.Dispose();
            MarkedButton.Dispose();
            UnmarkedButton.Dispose();
        }

        public void Render()
        {
            if (IsMarked)
                MarkedButton.Render();
            else
                UnmarkedButton.Render();
            ButtonText.Render();
        }

        public void Update()
        {
            if (FastUtils.IsNumberBetweenInterval(Input.Xpos, (Position.X, Position.X + Size.X)) &&
                FastUtils.IsNumberBetweenInterval(Input.Ypos, (Position.Y, Position.Y + Size.Y)))
            {
                IsMarked = true;
                if (Input.buttonPressed(TgcD3dInput.MouseButtons.BUTTON_LEFT))
                    Action();
            }
            else
                IsMarked = false;
        }
    }

    class DrawMenu
    {
        private List<DrawButton> Buttons;
        private TgcD3dInput Input;
        private string MediaDir;

        public DrawMenu(string mediaDir, TgcD3dInput input)
        {
            MediaDir = mediaDir;
            Input = input;
            Buttons = new List<DrawButton>();
        }

        public void CreateButton(string text, TGCVector2 scale, TGCVector2 position, Action action)
        {
            var button = new DrawButton(MediaDir, Input);
            button.InitializerButton(text, scale, position, action);
            Buttons.Add(button);
        }

        public void Dispose() => Buttons.ForEach(button => button.Dispose());
        public void Update() => Buttons.ForEach(button => button.Update());
        public void Render() => Buttons.ForEach(button => button.Render());
    }
}
