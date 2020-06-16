using System.Windows.Forms;
using TGC.Core.Sound;
using TGC.Group.Model.Objects;

namespace TGC.Group.Model
{
    class GameSoundManager
    {
        public TgcMp3Player Menu { get; private set; }
        public TgcMp3Player Ambient { get; private set; }
        public TgcStaticSound SharkEntrance { get; private set; }
        public TgcStaticSound CatchFish { get; private set; }
        public TgcStaticSound Crafting { get; private set; }
        public TgcStaticSound SharkDead { get; private set; }
        public TgcStaticSound Collect { get; private set; }
        public TgcStaticSound Equip { get; private set; }

        private string AmbientFileName;
        private string UnderWaterFileName;
        private bool JustSubmerge;

        public GameSoundManager(string mediaDir, TgcDirectSound sound)
        {
            Menu = new TgcMp3Player();
            Ambient = new TgcMp3Player();
            SharkEntrance = new TgcStaticSound();
            CatchFish = new TgcStaticSound();
            Crafting = new TgcStaticSound();
            SharkDead = new TgcStaticSound();
            Collect = new TgcStaticSound();
            Equip = new TgcStaticSound();
            Init(mediaDir, sound);
        }

        private void Init(string mediaDir, TgcDirectSound sound)
        {
            Menu.FileName = mediaDir + @"\Sounds\Menu.mp3";
            AmbientFileName = mediaDir + @"\Sounds\Ambient.mp3";
            UnderWaterFileName = mediaDir + @"\Sounds\UnderWater.mp3";
            SharkEntrance.loadSound(mediaDir + @"\Sounds\SharkNear.wav", sound.DsDevice);
            CatchFish.loadSound(mediaDir + @"\Sounds\CatchFish.wav", sound.DsDevice);
            Crafting.loadSound(mediaDir + @"\Sounds\Crafting.wav", sound.DsDevice);
            SharkDead.loadSound(mediaDir + @"\Sounds\SharkDead.wav", sound.DsDevice);
            Collect.loadSound(mediaDir + @"\Sounds\gather_resource.wav", sound.DsDevice);
            Equip.loadSound(mediaDir + @"\Sounds\Equip.wav", sound.DsDevice);
        }

        public void Dispose()
        {
            SharkEntrance.dispose();
            CatchFish.dispose();
            Crafting.dispose();
            SharkDead.dispose();
            Collect.dispose();
            Equip.dispose();
            Dispose(Menu);
            Dispose(Ambient);
        }

        public void PlayMusicAmbient(bool submerge)
        {    
            if (submerge)
            {
                if (JustSubmerge)
                {
                    JustSubmerge = false;
                    Ambient.stop();
                    Dispose(Ambient);
                    Ambient.FileName = UnderWaterFileName;
                    Ambient.play(true);
                }
            }
            else
            {
                if (!JustSubmerge)
                {
                    JustSubmerge = true;
                    Ambient.stop();
                    Dispose(Ambient);
                    Ambient.FileName = AmbientFileName;
                    Ambient.play(true);
                }
            }
        }

        public void Dispose(TgcMp3Player music)
        {
            if(music.FileName != null)
                music.closeFile();
        }
    }
}
