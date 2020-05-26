﻿using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Utils
{
    class CameraFPS : TgcCamera
    {
        public new TGCVector3 Position;
        public TGCVector3 Direction { get { return TGCVector3.Normalize(LookAt - Position); } }

        private struct Constants
        {
            public static float LIIMIT_MAX = FastMath.ToRad(60);
            public static float LIMIT_MIN = FastMath.ToRad(-60);
            public static Point MOUSE_CENTER = new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
            public static float ROTATION_SPEED = 0.1f;
            public static TGCVector3 DIRECTION_VIEW = new TGCVector3(0, 0.1f, -1);
        }

        private readonly TgcD3dInput Input;
        private TGCMatrix CameraRotation;
        public float Longitude { get; private set; } = -FastMath.PI / 10.0f;
        public float Latitude { get; private set; } = FastMath.PI_HALF;
        public bool Lock { get; set; }

        public CameraFPS(TgcD3dInput input)
        {
            Input = input;
            var d3Instance = D3DDevice.Instance;
            d3Instance.Device.Transform.Projection = TGCMatrix.PerspectiveFovLH(d3Instance.FieldOfView, d3Instance.AspectRatio,
                                                     d3Instance.ZNearPlaneDistance, d3Instance.ZFarPlaneDistance * 3f).ToMatrix();
        }

        public override void UpdateCamera(float elapsedTime)
        {
            if (Lock)
                return;

            Cursor.Hide();
            Rotation();
            var target = TGCVector3.TransformNormal(Constants.DIRECTION_VIEW, CameraRotation);
            var targetPosition = Position + target;
            var rotacionVectorUP = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, CameraRotation);
            Cursor.Position = Constants.MOUSE_CENTER;
            base.SetCamera(Position, targetPosition, rotacionVectorUP);
        }
    
        private void Rotation()
        {
            Latitude -= -Input.XposRelative * Constants.ROTATION_SPEED;
            Longitude -= Input.YposRelative * Constants.ROTATION_SPEED;
            Longitude = FastMath.Clamp(Longitude, Constants.LIMIT_MIN, Constants.LIIMIT_MAX);

            CameraRotation = TGCMatrix.RotationX(Longitude) * TGCMatrix.RotationY(Latitude);
        }
    }
}