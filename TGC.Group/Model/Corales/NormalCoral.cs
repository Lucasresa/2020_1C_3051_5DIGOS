using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Corales
{
    class NormalCoral : Coral
    {
        public NormalCoral(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "coral-TgcScene.xml";
        }


    }
}
