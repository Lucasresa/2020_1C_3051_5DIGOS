using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Core.Text;

namespace TGC.Group.Model.Draw
{
    class CharacterStatus
    {
        struct Constants
        {
            public static (float min, float max) oxygen = (min: 0, max: 100);
            public static (float min, float max) life = (min: 0, max: 100);
            public static (int width, int height) screen = (width: D3DDevice.Instance.Device.Viewport.Width, height: D3DDevice.Instance.Device.Viewport.Height);
        }

        private Bar life;
        private Bar oxygen;

        public bool canBreathe { get; set; }

        private string MediaDir;
        private string ShadersDir;

        private float oxygenPercentage = 100;
        private float lifePercentage = 100;

        private TgcText2D DrawText = new TgcText2D();

        public CharacterStatus(string mediaDir, string shadersDir)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            initializer();
        }

        private void initializer()
        {
            life = new Bar(MediaDir, ShadersDir);
            life.setInitialSprite(new TGCVector2(0.4f, 0.5f), new TGCVector2(100, 0), "barra_vida.png");

            oxygen = new Bar(MediaDir, ShadersDir);
            oxygen.setInitialSprite(new TGCVector2(0.4f, 0.5f), new TGCVector2(100, 30), "barra_oxigeno.png");
        }

        public void Update()
        {
            UpdateOxygen();
        }

        private void UpdateOxygen()
        {
            if (canRecoverOxygen())
                oxygenPercentage += 0.2f;

            oxygenPercentage -= 0.05f;
            oxygenPercentage = FastMath.Clamp(oxygenPercentage, Constants.oxygen.min, Constants.oxygen.max);

            var initialScale = oxygen.initialScaleSprite;
            var newScale = new TGCVector2((oxygenPercentage / Constants.oxygen.max) * initialScale.X, initialScale.Y);
            oxygen.sprite.Scaling = newScale;
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

            if (isDead()) DrawText.drawText("You are dead!", Constants.screen.width / 2, Constants.screen.height / 2, Color.Red); ;
        }

        public void Dispose()
        {
            life.Dispose();
            oxygen.Dispose();
        }

        private bool isDead()
        {
            return oxygenPercentage == 0;
        }
    }
}
