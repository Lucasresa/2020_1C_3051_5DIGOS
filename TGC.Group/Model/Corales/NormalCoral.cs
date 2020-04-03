using TGC.Core.Mathematica;

namespace TGC.Group.Model.Corales
{
    class NormalCoral : Coral
    {
        public NormalCoral(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "coral_normal-TgcScene.xml";
        }
    }
}
