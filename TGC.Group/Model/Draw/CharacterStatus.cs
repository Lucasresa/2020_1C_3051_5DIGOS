using Microsoft.DirectX.DirectInput;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Core.Direct3D;
using TGC.Core.Input;
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

        private Sprite life;
        private Sprite oxygen;
        private Sprite lookAt;

        public bool canBreathe { get; set; }

        private string MediaDir;
        private string ShadersDir;

        private float oxygenPercentage = 100;
        private float lifePercentage = 100;
        public TgcD3dInput input;
        private TgcText2D DrawText = new TgcText2D();

        public CharacterStatus(string mediaDir, string shadersDir, TgcD3dInput input)
        {
            MediaDir = mediaDir;
            ShadersDir = shadersDir;
            this.input = input;
            initializer();
        }

        private void initializer()
        {
            life = new Sprite(MediaDir, ShadersDir);
            life.setInitialSprite(new TGCVector2(0.4f, 0.5f), new TGCVector2(100, 0), "barra_vida");

            oxygen = new Sprite(MediaDir, ShadersDir);
            oxygen.setInitialSprite(new TGCVector2(0.4f, 0.5f), new TGCVector2(100, 30), "barra_oxigeno");
            
            lookAt = new Sprite(MediaDir, ShadersDir);
            lookAt.setInitialSprite(new TGCVector2(1, 1), "mira");
            var positionX = (Constants.screen.width - lookAt.sprite.texture.Size.Width) / 2;
            var positionY = (Constants.screen.height - lookAt.sprite.texture.Size.Height) / 2;
            lookAt.sprite.Position = new TGCVector2(positionX, positionY);
        }

        public void Update()
        {
            if (!isDead())
            {
                UpdateOxygen();
                UpdateLife();
            }
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

        private void UpdateLife()
        {
            var damage = 5f; // TODO: ver como contamos el daño, si recibe un daño fijo o depende de algo
            if (receivedAnAttack())
            {
                lifePercentage -= damage;
                lifePercentage = FastMath.Clamp(lifePercentage, Constants.life.min, Constants.life.max);

                var initialScale = life.initialScaleSprite;
                var newScale = new TGCVector2((lifePercentage / Constants.life.max) * initialScale.X, initialScale.Y);
                life.sprite.Scaling = newScale;
            }
        }

        private bool receivedAnAttack()
        {
            // TODO: Podriamos decir que recibio un ataque si el tiburon colisiono o se acerco a cierta distancia al personaje
            if (input.keyPressed(Key.Q))
                return true;
            else
                return false;

        }

        private bool canRecoverOxygen()
        {
            return canBreathe && !isDead();
        }

        public void Render()
        {
            life.Render();
            oxygen.Render();
            lookAt.Render();
            life.drawText("LIFE", Color.MediumVioletRed, new Point(10, 20), new Size(100, 100), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 14, FontStyle.Bold));
            oxygen.drawText("OXYGEN", Color.DeepSkyBlue, new Point(10, 50), new Size(100, 100), TgcText2D.TextAlign.LEFT, new Font("Arial Black", 14, FontStyle.Bold));

            if (isDead()) DrawText.drawText("You are dead!", Constants.screen.width / 2, Constants.screen.height / 2, Color.Red); ;
        }

        public void Dispose()
        {
            life.Dispose();
            oxygen.Dispose();
            lookAt.Dispose();
        }

        private bool isDead()
        {            
            return oxygenPercentage == 0 || lifePercentage == 0;
        }
    }
}
