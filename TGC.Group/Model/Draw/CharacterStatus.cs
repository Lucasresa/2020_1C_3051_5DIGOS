using Microsoft.DirectX.DirectInput;
using System.Drawing;
using TGC.Core.Direct3D;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model.Draw
{
    class CharacterStatus
    {
        #region Atributos
        struct Constants
        {
            public static (float min, float max) oxygen = (min: 0, max: 100);
            public static (float min, float max) life = (min: 0, max: 100);
            public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);
        }

        private TgcText2D DrawText = new TgcText2D();
        private Sprite life, oxygen;
        private string MediaDir, ShadersDir;
        public float oxygenPercentage = 100, lifePercentage = 100;

        public TgcD3dInput input;
        public bool canBreathe;
        #endregion

        #region Constructor
        public CharacterStatus(string mediaDir, string shadersDir, TgcD3dInput input)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            this.input = input;
            initializer();
        }
        #endregion

        #region Metodos
        private void initializer()
        {
            life = new Sprite(MediaDir, ShadersDir);
            life.setInitialSprite(new TGCVector2(0.4f, 0.5f), new TGCVector2(100, 0), "barra_vida");

            oxygen = new Sprite(MediaDir, ShadersDir);
            oxygen.setInitialSprite(new TGCVector2(0.4f, 0.5f), new TGCVector2(100, 30), "barra_oxigeno");
        }

        public void Update(bool hasADivingHelmet)
        {
            if (isDead())
                return;

            if (canRecoverOxygen())
                oxygenPercentage += 1f;

            if (hasADivingHelmet)
                oxygenPercentage -= 0.025f;
            else
                oxygenPercentage -= 0.05f;

            oxygenPercentage = FastMath.Clamp(oxygenPercentage, Constants.oxygen.min, Constants.oxygen.max);

            var initialScale = oxygen.initialScaleSprite;
            var newScale = new TGCVector2((oxygenPercentage / Constants.oxygen.max) * initialScale.X, initialScale.Y);
            oxygen.sprite.Scaling = newScale;

            if (isDead())
            {
                lifePercentage = 100;
                lifePercentage = FastMath.Clamp(lifePercentage, Constants.life.min, Constants.life.max);

                initialScale = life.initialScaleSprite;
                newScale = new TGCVector2((lifePercentage / Constants.life.max) * initialScale.X, initialScale.Y);
                life.sprite.Scaling = newScale;
            }


        }

        public void ReceiveDamage(float damage)
        {
            lifePercentage -= damage;
            lifePercentage = FastMath.Clamp(lifePercentage, Constants.life.min, Constants.life.max);

            var initialScale = life.initialScaleSprite;
            var newScale = new TGCVector2((lifePercentage / Constants.life.max) * initialScale.X, initialScale.Y);
            life.sprite.Scaling = newScale;
        }

        private bool canRecoverOxygen()
        {
            return canBreathe && !isDead();
        }

        public void Render()
        {
            life.Render();
            oxygen.Render();
            life.drawText("LIFE", Color.MediumVioletRed, new Point(10, 20), new Size(100, 100), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 14, FontStyle.Bold));
            oxygen.drawText("OXYGEN", Color.DeepSkyBlue, new Point(10, 50), new Size(100, 100), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 14, FontStyle.Bold));

            if (isDead())
            {
                DrawText.drawText("You are dead!", Constants.screen.width / 2, Constants.screen.height / 2, Color.Red);
                lifePercentage = 100;
                lifePercentage = FastMath.Clamp(lifePercentage, Constants.life.min, Constants.life.max);

                var initialScale = life.initialScaleSprite;
                var newScale = new TGCVector2((lifePercentage / Constants.life.max) * initialScale.X, initialScale.Y);
                life.sprite.Scaling = newScale;
            }
        }

        public void Dispose()
        {
            life.Dispose();
            oxygen.Dispose();
        }

        public bool isDead()
        {
            return oxygenPercentage == 0 || lifePercentage == 0;
        }
        #endregion
    }
}