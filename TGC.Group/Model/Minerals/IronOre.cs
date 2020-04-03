using TGC.Core.Mathematica;

namespace TGC.Group.Model.Minerals
{
    class IronOre : Mineral
    {
        public IronOre(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "Hierro-TgcScene.xml";
        }
    }
}
