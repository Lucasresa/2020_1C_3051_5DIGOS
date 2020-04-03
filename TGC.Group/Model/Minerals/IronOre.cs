using TGC.Core.Mathematica;

namespace TGC.Group.Model.Minerals
{
    class IronOre : Ore
    {
        public IronOre(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "iron-TgcScene.xml";
        }
    }
}
