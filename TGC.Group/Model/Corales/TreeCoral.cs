using TGC.Core.Mathematica;

namespace TGC.Group.Model.Corales
{
    class TreeCoral : Coral
    {
        public TreeCoral(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "tree_coral-TgcScene.xml";
        }
    }
}
