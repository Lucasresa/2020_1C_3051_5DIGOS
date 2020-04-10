using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;

namespace TGC.Group.Model.Corales
{
    abstract class Coral : CommonMesh
    {
        public Coral(string mediaDir, TGCVector3 center, string meshName) : base(mediaDir, center, meshName)
        {}

    }
}
