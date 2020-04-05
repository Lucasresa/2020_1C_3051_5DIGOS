using TGC.Core.Mathematica;

namespace TGC.Group.Model.Minerals
{
    class Rock : Ore
    {
        public Rock(string mediaDir, TGCVector3 position, string meshName) : base(mediaDir, position, meshName)
        {
        }
    }
}
