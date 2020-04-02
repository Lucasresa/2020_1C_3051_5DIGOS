using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Corales
{
    class SpiralCoral : Coral
    {
        public SpiralCoral(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "spiral_wire_coral-TgcScene.xml";
        }

        public override void Init()
        {
            base.Init();
            Mesh.Scale = new TGCVector3(10.5f, 10.5f, 10.5f);
        }
    }
}
