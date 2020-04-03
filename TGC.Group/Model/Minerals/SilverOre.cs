using TGC.Core.Mathematica;

namespace TGC.Group.Model.Minerals
{
    class SilverOre : Ore
    {
        public SilverOre(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "silver-TgcScene.xml";
        }        
    }
}
