using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Minerals
{
    abstract class Ore : CommonMesh
    {
        public Ore(string mediaDir, TGCVector3 center, string meshName) : base(mediaDir, center, meshName)
        {}
    }
}
