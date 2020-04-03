using TGC.Core.Mathematica;

namespace TGC.Group.Model.Corales
{
    class SpiralCoral : Coral
    {
        public SpiralCoral(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "spiral_wire_coral-TgcScene.xml";
        }
    }
}
