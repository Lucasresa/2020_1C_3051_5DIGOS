using TGC.Core.Mathematica;

namespace TGC.Group.Model.Minerals
{
    class GoldOre : Mineral
    {
        public GoldOre(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "Oro-TgcScene.xml";
        }
    }
}
