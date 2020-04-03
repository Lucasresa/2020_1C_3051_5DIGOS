using TGC.Core.Mathematica;

namespace TGC.Group.Model.Minerals
{
    class GoldOre : Ore
    {
        public GoldOre(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "gold-TgcScene.xml";
        }
    }
}
