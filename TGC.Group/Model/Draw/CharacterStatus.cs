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
            public static float LIFE_REDUCE_STEP = 0.3f;
        }

        private TgcText2D DrawText = new TgcText2D();
        private Sprite life, oxygen;
        private string MediaDir, ShadersDir;
        private float DamageAcumulated = 0;
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

            UpdateOxygenSprite();
            UpdateLifeSprite();
        }

        public void ReceiveDamage(float damage)
        {
            DamageAcumulated = damage;
        }

        public void Render()
        {
            life.Render();
            oxygen.Render();
            life.drawText("LIFE", Color.MediumVioletRed, new Point(10, 20), new Size(100, 100), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 14, FontStyle.Bold));
            oxygen.drawText("OXYGEN", Color.DeepSkyBlue, new Point(10, 50), new Size(100, 100), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 14, FontStyle.Bold));

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

        private bool canRecoverOxygen()
        {
            return canBreathe && !isDead();
        }

        private void UpdateLifeSprite()
        {
            if (DamageAcumulated > 0)
            {
                lifePercentage -= Constants.LIFE_REDUCE_STEP;
                lifePercentage = FastMath.Clamp(lifePercentage, Constants.life.min, Constants.life.max);

                var initialScale = life.initialScaleSprite;
                var newScale = new TGCVector2((lifePercentage / Constants.life.max) * initialScale.X, initialScale.Y);
                life.sprite.Scaling = newScale;
                DamageAcumulated -= Constants.LIFE_REDUCE_STEP;
            }
        }

        private void UpdateOxygenSprite()
        {
            oxygenPercentage = FastMath.Clamp(oxygenPercentage, Constants.oxygen.min, Constants.oxygen.max);

            var initialScale = oxygen.initialScaleSprite;
            var newScale = new TGCVector2((oxygenPercentage / Constants.oxygen.max) * initialScale.X, initialScale.Y);
            oxygen.sprite.Scaling = newScale;
        }

        #endregion
    }
}