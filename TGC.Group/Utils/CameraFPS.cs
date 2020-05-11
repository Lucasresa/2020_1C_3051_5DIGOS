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

        private struct Constants
        {
            public static TGCVector3 outdoorPosition = new TGCVector3(1300, 3505, 20);
            public static TGCVector3 indoorPosition  = new TGCVector3(515, -2340, -40);
        }

        public float rotationSpeed = 0.1f;
        public float movementSpeed = 500f;
        public float jumpSpeed = 500f;
        public TGCVector3 position;
        public bool lockCam = false;
        private TgcD3dInput Input { get; }
        private Point mouseCenter = new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
        private TGCMatrix cameraRotation;
        public float Latitude { get; private set; } = FastMath.PI_HALF;
        private float longitude = -FastMath.PI / 10.0f;
        private TGCVector3 directionView = new TGCVector3(0, 0.1f, -1);
                
        private float limitMax = FastMath.ToRad(90);
        private float limitMin = FastMath.ToRad(-60);

         #endregion

        #region Constructores

        public CameraFPS(TgcD3dInput input)
        {
            position = Constants.indoorPosition;
            Cursor.Hide();
            Input = input;
            cameraRotation = TGCMatrix.RotationX(Latitude) * TGCMatrix.RotationY(longitude);
        }

        #endregion             
        
        #region Rotacion de la Camara

        private void CameraRotation()
        {
            Latitude -= -Input.XposRelative * rotationSpeed;
            longitude -= Input.YposRelative * rotationSpeed;
            longitude = FastMath.Clamp(longitude, limitMin, limitMax);

            cameraRotation = TGCMatrix.RotationX(longitude) * TGCMatrix.RotationY(Latitude);
        }
        #endregion

        public override void UpdateCamera(float elapsedTime)
        {
            if (Input.keyPressed(Key.I))
                lockCam = !lockCam;

            if (lockCam)
                return;

            Cursor.Hide();
            CameraRotation();
            var target = TGCVector3.TransformNormal(directionView, cameraRotation);
            var targetPosition = position + target;
            var rotacionVectorUP = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, cameraRotation);
            Cursor.Position = mouseCenter;
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
    }
}