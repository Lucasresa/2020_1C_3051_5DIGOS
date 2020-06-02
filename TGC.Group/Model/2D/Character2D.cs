using System.Collections.Generic;
using System.Linq;
using TGC.Core.Direct3D;
using TGC.Core.Mathematica;
using TGC.Group.Model.Status;
using TGC.Group.Utils;

namespace TGC.Group.Model._2D
{
    class Character2D
    {
        private struct Constants
        {
            public static int SCREEN_WIDTH = D3DDevice.Instance.Device.Viewport.Width;
            public static int SCREEN_HEIGHT = D3DDevice.Instance.Device.Viewport.Height;
            public static TGCVector2 LIFE_CHARACTER_SCALE = new TGCVector2(0.3f, 0.45f);
            public static TGCVector2 LIFE_CHARACTER_POSITION = new TGCVector2(20, SCREEN_HEIGHT - 80);
            public static TGCVector2 LIFE_CHARACTER_TEXT_SIZE = new TGCVector2(150, 50);
            public static TGCVector2 LIFE_CHARACTER_TEXT_POSITION = new TGCVector2(((1000 * LIFE_CHARACTER_SCALE.X) - LIFE_CHARACTER_TEXT_SIZE.X + 20) / 2, LIFE_CHARACTER_POSITION.Y + 15);
            public static TGCVector2 OXYGEN_CHARACTER_POSITION = new TGCVector2(20, LIFE_CHARACTER_POSITION.Y + 25);
            public static TGCVector2 OXYGEN_CHARACTER_SCALE = new TGCVector2(0.3f, 0.45f);
            public static TGCVector2 OXYGEN_CHARACTER_TEXT_SIZE = new TGCVector2(150, 50);
            public static TGCVector2 OXYGEN_CHARACTER_TEXT_POSITION = new TGCVector2(((1000 * OXYGEN_CHARACTER_SCALE.X) - OXYGEN_CHARACTER_TEXT_SIZE.X + 20) / 2, OXYGEN_CHARACTER_POSITION.Y + 15);
        }

        private readonly DrawSprite LifeCharacter;
        private readonly DrawSprite OxygenCharacter;
        private readonly DrawText LifeCharacterText;
        private readonly DrawText OxygenCharacterText;
        private CharacterStatus Status { get; set; }

        public Character2D(string MediaDir, CharacterStatus status)
        {
            Status = status;
            LifeCharacter = new DrawSprite(MediaDir);
            OxygenCharacter = new DrawSprite(MediaDir);
            LifeCharacterText = new DrawText();
            OxygenCharacterText = new DrawText();
            Init();
        }

        public void Dispose()
        {
            LifeCharacter.Dispose();
            LifeCharacterText.Dispose();
            OxygenCharacter.Dispose();
            OxygenCharacterText.Dispose();
        }

        public void Init()
        {
            InitializerLifeCharacter();
            InitializerOxygenCharacter();
        }

        private void InitializerLifeCharacter()
        {
            LifeCharacter.SetImage("LifeBar.png");
            LifeCharacter.SetInitialScallingAndPosition(Constants.LIFE_CHARACTER_SCALE, Constants.LIFE_CHARACTER_POSITION);
            LifeCharacterText.Size = Constants.LIFE_CHARACTER_TEXT_SIZE;
        }

        private void InitializerOxygenCharacter()
        {
            OxygenCharacter.SetImage("OxygenBar.png");
            OxygenCharacter.SetInitialScallingAndPosition(Constants.OXYGEN_CHARACTER_SCALE, Constants.OXYGEN_CHARACTER_POSITION);
            OxygenCharacterText.Size = new TGCVector2(Constants.OXYGEN_CHARACTER_TEXT_SIZE.X + 300, Constants.OXYGEN_CHARACTER_TEXT_SIZE.Y);
        }

        public void Render()
        {
            LifeCharacter.Render();
            OxygenCharacter.Render();
            LifeCharacterText.SetTextAndPosition(text: " Life   " + Status.ShowLife + @" \ " + Status.GetLifeMax(),
                                                 position: Constants.LIFE_CHARACTER_TEXT_POSITION);
            OxygenCharacterText.SetTextAndPosition(text: "    O₂    " + Status.ShowOxygen + @" \ " + Status.GetOxygenMax(),
                                                   position: Constants.OXYGEN_CHARACTER_TEXT_POSITION);
            LifeCharacterText.Render();
            OxygenCharacterText.Render();
        }

        public void Update()
        {
            UpdateSprite(LifeCharacter, Status.Life, Status.GetLifeMax());
            UpdateSprite(OxygenCharacter, Status.Oxygen, Status.GetOxygenMax());
        }

        private void UpdateSprite(DrawSprite sprite, float percentage, float max) => sprite.Scaling = new TGCVector2((percentage / max) * sprite.ScalingInitial.X, sprite.ScalingInitial.Y);
    }
}
