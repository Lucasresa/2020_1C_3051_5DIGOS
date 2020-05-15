using Microsoft.DirectX.DirectInput;
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

        public float rotationSpeed = 0.1f;
        public float movementSpeed = 500f;
        public float jumpSpeed = 500f;
        public TGCVector3 position;
        public TGCVector3 Direction { get { return TGCVector3.Normalize(LookAt - position); } }
        public bool isOutside = false;

        private TgcD3dInput Input { get; }
        private Point mouseCenter = new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
        private TGCMatrix cameraRotation;
        private float latitude = FastMath.PI_HALF;
        private float longitude = -FastMath.PI / 10.0f;
        private TGCVector3 directionView = new TGCVector3(0, 0.1f, -1);
        private TGCVector3 translation = TGCVector3.Empty;

        private TGCVector3 indoorPosition;
        private TGCVector3 outdoorPosition;

        protected TGCVector3 moveX = new TGCVector3(1, 0, 0);
        protected TGCVector3 moveY = new TGCVector3(0, 1, 0);
        protected TGCVector3 moveZ = new TGCVector3(0, 0, 1);
        protected float limitMax = FastMath.ToRad(90);
        protected float limitMin = FastMath.ToRad(-60);

        public float Latitude { get { return latitude; } }

        #endregion

        #region Constructores

        public CameraFPS(TgcD3dInput input, TGCVector3 indoorPosition, TGCVector3 outdoorPosition)
        {
            Input = input;
            setShipPosition(indoorPosition, outdoorPosition);
            cameraRotation = TGCMatrix.RotationX(latitude) * TGCMatrix.RotationY(longitude);
        }

        #endregion

        public void setShipPosition(TGCVector3 inside, TGCVector3 outside)
        {
            indoorPosition = inside;
            outdoorPosition = outside;
        }

        public TGCVector3 getIndoorPosition()
        {
            return indoorPosition;
        }

        public TGCVector3 getOutdoorPosition()
        {
            return outdoorPosition;
        }
        
        #region Desplazamiento de la Camara
        private void CameraTranslate()
        {
            if (Input.keyDown(Key.W)) translation += moveZ * -movementSpeed;

            if (Input.keyDown(Key.S)) translation += moveZ * movementSpeed;

            if (Input.keyDown(Key.D)) translation += moveX * -movementSpeed;

            if (Input.keyDown(Key.A)) translation += moveX * movementSpeed;

            if (Input.keyDown(Key.Space)) translation += moveY * jumpSpeed;

            if (Input.keyDown(Key.LeftControl)) translation += moveY * -jumpSpeed;

        }
        #endregion

        #region Rotacion de la Camara

        private void CameraRotation()
        {
            latitude -= -Input.XposRelative * rotationSpeed;
            longitude -= Input.YposRelative * rotationSpeed;
            longitude = FastMath.Clamp(longitude, limitMin, limitMax);

            cameraRotation = TGCMatrix.RotationX(longitude) * TGCMatrix.RotationY(latitude);
        }
        #endregion

        public override void UpdateCamera(float elapsedTime)
        {
            if (position.Y < 0)
                isOutside = false;
            else
                isOutside = true;

            Cursor.Hide();
            CameraRotation();

            var newPosition = TGCVector3.TransformNormal(translation * elapsedTime, cameraRotation);
            position += newPosition;

            translation = TGCVector3.Empty;

            var target = TGCVector3.TransformNormal(directionView, cameraRotation);
            var targetPosition = position + target;

            var rotacionVectorUP = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, cameraRotation);

            Cursor.Position = mouseCenter;
            base.SetCamera(position, targetPosition, rotacionVectorUP);
        }

        public void TeleportCamera(TGCVector3 translatePosition)
        {
            position = translatePosition;

            var target = TGCVector3.TransformNormal(directionView, cameraRotation);
            var targetPosition = position + target;

            base.SetCamera(position, targetPosition);
        }

    }
}