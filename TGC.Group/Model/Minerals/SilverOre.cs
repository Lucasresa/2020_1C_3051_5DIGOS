using TGC.Core.Mathematica;

namespace TGC.Group.Model.Minerals
{
    class SilverOre : Mineral
    {
        public SilverOre(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "Plata-TgcScene.xml";
        }        
    }
}
