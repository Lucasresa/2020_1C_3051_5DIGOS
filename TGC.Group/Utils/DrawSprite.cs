using Microsoft.DirectX.Direct3D;
using System;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Utils
{
    class DrawSprite
    {
        private readonly string MediaDir;
        private Sprite Sprite { get; set; }
        private Rectangle SrcRect { get; set; }
        private TgcTexture Texture { get; set; }
        private TGCMatrix TransformationMatrix { get; set; }

        public Color Color { get; set; }
        public TGCVector2 Position { get { return Position; } set { Position = value; UpdateTransformationMatrix(); } }
        public float Rotation { get { return Rotation; } set { Rotation = value; UpdateTransformationMatrix(); } }
        public TGCVector2 RotationCenter { get { return RotationCenter; } set { RotationCenter = value; UpdateTransformationMatrix(); } }
        public TGCVector2 Scaling { get { return Scaling; } set { Scaling = value; UpdateTransformationMatrix(); } }
        public TGCVector2 ScalingCenter { get { return ScalingCenter; } set { ScalingCenter = value; UpdateTransformationMatrix(); } }
        public TGCVector2 ScalingInitial { get; private set; }

        public DrawSprite(string mediaDir)
        {
            MediaDir = mediaDir;
            Initialize();
        }

        public void BeginDraw()
        {
            Sprite.Begin(SpriteFlags.AlphaBlend | SpriteFlags.SortDepthFrontToBack);
        }

        public void Dispose()
        {
            if (Texture != null) Texture.dispose();
            Sprite.Dispose();
        }

        public void Draw()
        {
            Sprite.Transform = TransformationMatrix.ToMatrix();
            Sprite.Draw(Texture.D3dTexture, SrcRect, TGCVector3.Empty, TGCVector3.Empty, Color);
        }

        public void EndDraw()
        {
            Sprite.End();
        }

        private void Initialize()
        {
            TransformationMatrix = TGCMatrix.Identity;
            SrcRect = Rectangle.Empty;
            Position = TGCVector2.Zero;
            ScalingInitial = Scaling = TGCVector2.One;
            ScalingCenter = TGCVector2.Zero;
            Rotation = 0;
            RotationCenter = TGCVector2.Zero;
            Color = Color.White;
            Sprite = new Sprite(D3DDevice.Instance.Device);
        }

        public void SetImage(string imageNameAndExtension)
        {
            try { Texture = TgcTexture.createTexture(MediaDir + @"Imagenes\" + imageNameAndExtension); }
            catch { throw new Exception("Sprite image file, not found!"); }    
        }

        public void SetInitialScallingAndPosition(TGCVector2 scale, TGCVector2 position)
        {
            Scaling = ScalingInitial = scale;
            Position = position;
        }

        private void UpdateTransformationMatrix()
        {
            TransformationMatrix = TGCMatrix.Transformation2D(ScalingCenter, 0, Scaling, RotationCenter, Rotation, Position);
        } 
    }
}