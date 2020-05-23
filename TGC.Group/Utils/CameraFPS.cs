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
        public new TGCVector3 Position;
        public TGCVector3 Direction { get { return TGCVector3.Normalize(LookAt - Position); } }

        private struct Constants
        {            
            public static float limitMax = FastMath.ToRad(60);
            public static float limitMin = FastMath.ToRad(-60);
            public static Point mouseCenter = new Point(D3DDevice.Instance.Device.Viewport.Width / 2, D3DDevice.Instance.Device.Viewport.Height / 2);
            public static float rotationSpeed = 0.1f;
            public static TGCVector3 directionView = new TGCVector3(0, 0.1f, -1);
        }

        private TgcD3dInput Input;
        private TGCMatrix CameraRotation;
        public float Longitude { get; private set; } = -FastMath.PI / 10.0f;
        public float Latitude { get; private set; }  = FastMath.PI_HALF;
    
        public CameraFPS(TgcD3dInput input)
        {
            Input = input;
        }
        
        public override void UpdateCamera(float elapsedTime)
        {
            Cursor.Hide();
            Rotation();
            var target = TGCVector3.TransformNormal(Constants.directionView, CameraRotation);
            var targetPosition = Position + target;
            var rotacionVectorUP = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, CameraRotation);
            Cursor.Position = Constants.mouseCenter;
            base.SetCamera(Position, targetPosition, rotacionVectorUP);
        }

        public bool isOutside()
        {
            return Position.Y > 0;
        }

        private void Rotation()
        {
            Latitude -= -Input.XposRelative * Constants.rotationSpeed;
            Longitude -= Input.YposRelative * Constants.rotationSpeed;
            Longitude = FastMath.Clamp(Longitude, Constants.limitMin, Constants.limitMax);

            CameraRotation = TGCMatrix.RotationX(Longitude) * TGCMatrix.RotationY(Latitude);
        }
    }
}