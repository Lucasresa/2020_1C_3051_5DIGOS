using Microsoft.DirectX.Direct3D;
using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.Text;
using TGC.Group.Utils;
using TGC.Core.Textures;
using TGC.Core.Direct3D;
using Font = System.Drawing.Font;

namespace TGC.Group.Model.Draw
{
    class Sprite
    {
        private string MediaDir;
        private string ShadersDir;

        public TgcText2D text;
        
        private Microsoft.DirectX.Direct3D.Sprite DxSprite;
        public CustomSprite sprite;
        public TGCVector2 initialScaleSprite { get; private set; }
            
        public Sprite(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            Init();
        }

        public Sprite()
        {
            text = new TgcText2D();
        }

        private void Init()
        {
            DxSprite = new Microsoft.DirectX.Direct3D.Sprite(D3DDevice.Instance.Device);
            sprite = new CustomSprite();
            text = new TgcText2D();
        }

        public void setInitialSprite(TGCVector2 scale, TGCVector2 position, string nameImage)
        {
            sprite.Scaling = initialScaleSprite = scale;
            sprite.Position = position;
            sprite.texture = TgcTexture.createTexture(MediaDir + @"Imagenes\" + nameImage + ".png");
        }

        public void setInitialSprite(TGCVector2 scale, string nameImage)
        {
            sprite.Scaling = initialScaleSprite = scale;
            sprite.texture = TgcTexture.createTexture(MediaDir + @"Imagenes\" + nameImage + ".png");
        }

        public void drawText(string Text, Color color, Point posicion, Size size, TgcText2D.TextAlign align, Font font)
        {
            text.Text = Text;
            text.Color = color;
            text.Align = align;
            text.Position = posicion;
            text.Size = size;
            text.changeFont(font);
            text.render();
        }

        public void Render()
        {
            if (sprite != null)
                DrawSprite(sprite);
        }

        public void Dispose()
        {
            if (text != null)
                text.Dispose();
            if (sprite != null)
            {
                sprite.Dispose();
                DxSprite.Dispose();
            }
        }

        public void DrawSprite(CustomSprite sprite)
        {
            DxSprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthFrontToBack);
            DxSprite.Transform = sprite.TransformationMatrix.ToMatrix();
            DxSprite.Draw(sprite.texture.D3dTexture, sprite.SrcRect, TGCVector3.Empty, TGCVector3.Empty, sprite.Color);
            DxSprite.End();
        }
    }
}