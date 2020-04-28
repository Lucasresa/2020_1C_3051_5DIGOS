using Microsoft.DirectX.Direct3D;
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
        #region Variables

        #region Publicas
        public float rotationSpeed { get; set; } = 0.1f;
        public float movementSpeed { get; set; } = 200f;
        public float jumpSpeed { get; set; } = 200f;
        public TGCVector3 position { get; set; } = new TGCVector3(1300, 3505, 20);
        #endregion
        
        #region Privadas
        private TgcD3dInput input { get; }
        private Point mouseCenter = new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
        private TGCMatrix cameraRotation;
        private float latitude { get; set; } = FastMath.PI_HALF;
        private float longitude { get; set; } = -FastMath.PI / 10.0f;
        private TGCVector3 directionView { get; set; } = new TGCVector3(0, 0.1f, -1);
        private TGCVector3 translation { get; set; } = TGCVector3.Empty;
        #endregion

        #region Protegidas
        protected TGCVector3 moveX = new TGCVector3(1, 0, 0);
        protected TGCVector3 moveY = new TGCVector3(0, 1, 0);
        protected TGCVector3 moveZ = new TGCVector3(0, 0, 1);
        protected float limitMax = FastMath.ToRad(90);
        protected float limitMin = FastMath.ToRad(-60);
        #endregion
        
        #endregion

        #region Constructores
        public CameraFPS(TgcD3dInput input)
        {
            this.input = input;
            cameraRotation = TGCMatrix.RotationX(latitude) * TGCMatrix.RotationY(longitude);
        }

        public CameraFPS(TgcD3dInput input, TGCVector3 pos) : this(input)
        {
            position = pos;
            cameraRotation = TGCMatrix.RotationX(latitude) * TGCMatrix.RotationY(longitude);
        }
        #endregion

        #region Desplazamiento de la Camara
        private void CameraTranslate()
        {
            if (input.keyDown(Key.W)) translation += moveZ * -movementSpeed;
                       
            if (input.keyDown(Key.S)) translation += moveZ * movementSpeed;
                      
            if (input.keyDown(Key.D)) translation += moveX * -movementSpeed;
                       
            if (input.keyDown(Key.A)) translation += moveX * movementSpeed;
                      
            if (input.keyDown(Key.Space)) translation += moveY * jumpSpeed;
                      
            if (input.keyDown(Key.LeftControl)) translation += moveY * -jumpSpeed;

        }
        #endregion

        #region Rotacion de la Camara

        private void CameraRotation()
        {
            latitude -= -input.XposRelative * rotationSpeed;
            longitude -= input.YposRelative * rotationSpeed;
            longitude = FastMath.Clamp(longitude, limitMin, limitMax);

            cameraRotation = TGCMatrix.RotationX(longitude) * TGCMatrix.RotationY(latitude);
        }
        #endregion

        public override void UpdateCamera(float elapsedTime)
        {
            Cursor.Hide();
            CameraTranslate();
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
