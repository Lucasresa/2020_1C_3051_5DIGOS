using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using Font = System.Drawing.Font;

namespace TGC.Group.Utils
{
    class DrawText
    { 
        private TgcText2D Text2D { get; set; }

        public string Text { get { return Text; } set { Text2D.Text = Text = value; } }
        public Color Color { get { return Color; } set { Text2D.Color = Color = value; } }
        public TGCVector2 Position { get { return Position; } set { Position = value; Text2D.Position = new Point((int)Position.X, (int)Position.Y); } }
        public TGCVector2 Scaling { get { return Scaling; } set { Scaling = value; Text2D.Size = new Size((int)Scaling.X, (int)Scaling.Y); } }
        public TgcText2D.TextAlign Align { get { return Align; } set { Text2D.Align = Align = value; } }
        public Font Font { get { return Font; } set { Font = value; Text2D.changeFont(Font); } }

        public DrawText()
        {
            Text2D = new TgcText2D();
            Initializer();
        }

        public void Dispose()
        {
            Text2D.Dispose();
        }        
        
        private void Initializer()
        {
            Text = "";
            Position = TGCVector2.Zero;
            Scaling = TGCVector2.One;
            Color = Color.White;
            Align = TgcText2D.TextAlign.LEFT;
            Font = new Font("Arial Black", 14, FontStyle.Bold);
            UpdateTextSettings();
        }

        public void Render()
        {
            Text2D.render();
        }

        public void SetTextScaleAndPosition(TGCVector2 position, TGCVector2 scalling)
        {
            Position = position;
            Scaling = scalling;
        }

        private void UpdateTextSettings()
        {
            Text2D.Text = Text;
            Text2D.Color = Color;
            Text2D.Align = Align;
            Text2D.Position = new Point((int)Position.X, (int)Position.Y);
            Text2D.Size = new Size((int)Scaling.X, (int)Scaling.Y);
            Text2D.changeFont(Font);
        }
    }
}
