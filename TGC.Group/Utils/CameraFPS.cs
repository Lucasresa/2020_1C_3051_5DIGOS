using Microsoft.DirectX.DirectInput;
using System;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Utils
{
    class CameraFPS : TgcCamera
    {
        #region Atributos

        public TGCVector3 position;
        public TGCVector3 Direction { get { return TGCVector3.Normalize(LookAt - position); } }

        private struct Constants
        {
            public static TGCVector3 outdoorPosition = new TGCVector3(1300, 3505, 20);
            public static TGCVector3 indoorPosition  = new TGCVector3(515, -2340, -40);
            public static float limitMax = FastMath.ToRad(60);
            public static float limitMin = FastMath.ToRad(-60);
            public static Point mouseCenter = new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
            public static float rotationSpeed = 0.1f;
            public static TGCVector3 directionView = new TGCVector3(0, 0.1f, -1);
        }

        private TgcD3dInput Input { get; }
        private TGCMatrix cameraRotation;
        public float longitude { get; private set; } = -FastMath.PI / 10.0f;
        public float latitude { get; private set; } = FastMath.PI_HALF;
        public bool lockCam;
        #endregion

        #region Constructores
        public CameraFPS(TgcD3dInput input)
        {
            position = Constants.indoorPosition;
            Input = input;
        }
        #endregion             
        
        public override void UpdateCamera(float elapsedTime)
        {
            if (lockCam)
                return; 
                
            Cursor.Hide();
            CameraRotation();
            var target = TGCVector3.TransformNormal(Constants.directionView, cameraRotation);
            var targetPosition = position + target;
            var rotacionVectorUP = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, cameraRotation);
            Cursor.Position = Constants.mouseCenter;
            base.SetCamera(position, targetPosition, rotacionVectorUP);
        }

        public TGCVector3 getIndoorPosition()
        {
            return Constants.indoorPosition;
        }

        public TGCVector3 getOutdoorPosition()
        {
            return Constants.outdoorPosition;
        }

        public bool isOutside()
        {
            return position.Y > 0;
        }

        #region Rotacion de la Camara
        private void CameraRotation()
        {
            latitude -= -Input.XposRelative * Constants.rotationSpeed;
            longitude -= Input.YposRelative * Constants.rotationSpeed;
            longitude = FastMath.Clamp(longitude, Constants.limitMin, Constants.limitMax);

            cameraRotation = TGCMatrix.RotationX(longitude) * TGCMatrix.RotationY(latitude);
        }
        #endregion
    }
}