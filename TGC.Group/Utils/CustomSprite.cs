using System;
using System.Drawing;
using TGC.Core.Mathematica;
using TGC.Core.Textures;

namespace TGC.Group.Utils
{
    public class CustomSprite : IDisposable
    {
        public TgcTexture texture { get; set; }
        public TGCMatrix TransformationMatrix { get; set; }
        public Rectangle SrcRect { get; set; }
        public Color Color { get; set; }

        private TGCVector2 position;
        public TGCVector2 Position
        {
            get { return position; }
            set
            {
                position = value;
                UpdateTransformationMatrix();
            }
        }

        private float rotation;
        public float Rotation
        {
            get { return rotation; }
            set
            {
                rotation = value;
                UpdateTransformationMatrix();
            }
        }

        private TGCVector2 rotationCenter;
        public TGCVector2 RotationCenter
        {
            get { return rotationCenter; }
            set
            {
                rotationCenter = value;
                UpdateTransformationMatrix();
            }
        }

        private TGCVector2 scalingCenter;
        public TGCVector2 ScalingCenter
        {
            get { return scalingCenter; }
            set
            {
                scalingCenter = value;
                UpdateTransformationMatrix();
            }
        }

        private TGCVector2 scaling;
        public TGCVector2 Scaling
        {
            get { return scaling; }
            set
            {
                scaling = value;
                UpdateTransformationMatrix();
            }
        }

        public CustomSprite()
        {
            initialize();
        }

        public void Dispose()
        {
            texture.dispose();
        }

        private void initialize()
        {
            TransformationMatrix = TGCMatrix.Identity;
            SrcRect = Rectangle.Empty;
            position = TGCVector2.Zero;
            scaling = TGCVector2.One;
            scalingCenter = TGCVector2.Zero;
            rotation = 0;
            rotationCenter = TGCVector2.Zero;
            Color = Color.White;
        }

        private void UpdateTransformationMatrix()
        {
            TransformationMatrix = TGCMatrix.Transformation2D(scalingCenter, 0, scaling, rotationCenter, rotation, position);
        }       
    }
}