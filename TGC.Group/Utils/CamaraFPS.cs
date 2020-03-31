using Microsoft.DirectX.DirectInput;
using System.Drawing;
using System.Windows.Forms;
using TGC.Core.Camara;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;

namespace TGC.Group.Utils
{
    class CamaraFPS : TgcCamera
    {
        #region Variables
        public const float VEL_ROTACION = 0.1f;
        public const float VEL_MOVIMIENTO = 200f;
        public const float VEL_SALTO = 200f;

        private TgcD3dInput Entrada { get; }
        public Point CentroMouse { get; }
        private TGCMatrix CamaraRotacional { get; set; }

        public float VelocidadRotacion { get; }
        public float VelocidadMovimiento { get; }
        public float VelocidadSalto { get; }
        private float Latitud { get; set; }
        private float Longitud { get; set; }
        
        private TGCVector3 Posicion { get; set; }
        private TGCVector3 DireccionDeLaVista { get; set; }
        private TGCVector3 MovimientoProducido { get; set; } = TGCVector3.Empty;
        private TGCVector3 MoverseEjeX { get; } = new TGCVector3(1, 0, 0);
        private TGCVector3 MoverseEjeY { get; } = new TGCVector3(0, 1, 0);
        private TGCVector3 MoverseEjeZ { get; } = new TGCVector3(0, 0, 1);
        #endregion

        #region Constructores
        public CamaraFPS(TgcD3dInput input)
        {
            Entrada = input;
            Posicion = new TGCVector3(0, 3505, 0);
            VelocidadRotacion = VEL_ROTACION;
            VelocidadMovimiento = VEL_MOVIMIENTO;
            VelocidadSalto = VEL_SALTO;
            var Pantalla = D3DDevice.Instance.Device.Viewport;
            CentroMouse = new Point(Pantalla.Width / 2, Pantalla.Height / 2);
            DireccionDeLaVista = new TGCVector3(0, 0.1f, -1);
            Latitud = FastMath.PI_HALF;
            Longitud = -FastMath.PI / 10.0f;
            CamaraRotacional = TGCMatrix.RotationX(Latitud) * TGCMatrix.RotationY(Longitud);
        }

        public CamaraFPS(TgcD3dInput input, TGCVector3 pos) : this(input)
        {
            Posicion = pos;
            VelocidadRotacion = VEL_ROTACION;
            VelocidadMovimiento = VEL_MOVIMIENTO;
            VelocidadSalto = VEL_SALTO;
            var Pantalla = D3DDevice.Instance.Device.Viewport;
            CentroMouse = new Point(Pantalla.Width / 2, Pantalla.Height / 2);
            DireccionDeLaVista = new TGCVector3(0, 0.1f, -1);
            Latitud = FastMath.PI_HALF;
            Longitud = -FastMath.PI / 10.0f;
            CamaraRotacional = TGCMatrix.RotationX(Latitud) * TGCMatrix.RotationY(Longitud);
        }
        #endregion
        
        #region Deteccion del movimiento
        private void DetectarMovimientoCamara()
        {
            if (Entrada.keyDown(Key.W)) MovimientoProducido += MoverseEjeZ * -VelocidadMovimiento;
                       
            if (Entrada.keyDown(Key.S)) MovimientoProducido += MoverseEjeZ * VelocidadMovimiento;
                      
            if (Entrada.keyDown(Key.D)) MovimientoProducido += MoverseEjeX * -VelocidadMovimiento;
                       
            if (Entrada.keyDown(Key.A)) MovimientoProducido += MoverseEjeX * VelocidadMovimiento;
                      
            if (Entrada.keyDown(Key.Space)) MovimientoProducido += MoverseEjeY * VelocidadSalto;
                      
            if (Entrada.keyDown(Key.LeftControl)) MovimientoProducido += MoverseEjeY * -VelocidadSalto;
            
        }
        #endregion
       
        #region Deteccion de la rotacion

        private void DetectarRotacionCamara()
        {
            Latitud -= -Entrada.XposRelative * VelocidadRotacion;

            float anguloLimite = 0.90f;

            var valor = LookAt.Y - Position.Y;

            bool dentroDelLimitante()
            {
                return valor < anguloLimite && valor > -anguloLimite;
            }
            
            if (dentroDelLimitante())
                Longitud -= Entrada.YposRelative * VelocidadRotacion;
            else
            {
                if (valor > anguloLimite)
                    Longitud -= 0.0025f * VelocidadRotacion;
                if (valor < -anguloLimite)
                    Longitud += 0.0025f * VelocidadRotacion;
            }

            CamaraRotacional = TGCMatrix.RotationX(Longitud) * TGCMatrix.RotationY(Latitud);
        }                
        #endregion

        public override void UpdateCamera(float elapsedTime)
        {
            Cursor.Hide();
            Cursor.Position = CentroMouse;
            DetectarMovimientoCamara();
            DetectarRotacionCamara();
            
            var MovimientoEnElTiempo = MovimientoProducido * elapsedTime;
            var nuevaPosicion = TGCVector3.TransformNormal(MovimientoEnElTiempo, CamaraRotacional);
            Posicion += nuevaPosicion;
            MovimientoProducido = TGCVector3.Empty;

            var objetivoCamara = TGCVector3.TransformNormal(DireccionDeLaVista, CamaraRotacional);
            var objetivoFinal = Posicion + objetivoCamara;

            var rotacionVectorUP = TGCVector3.TransformNormal(DEFAULT_UP_VECTOR, CamaraRotacional);

            base.SetCamera(Posicion, objetivoFinal, rotacionVectorUP);
        }
    }
}
