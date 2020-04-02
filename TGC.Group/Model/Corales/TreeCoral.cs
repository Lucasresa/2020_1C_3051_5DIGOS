using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Corales
{
    class TreeCoral : Coral
    {

        public TreeCoral(string mediaDir, TGCVector3 position) : base(mediaDir, position)
        {
            FILE_NAME = "tree_coral-TgcScene.xml";  
        }

        public override void Init()
        {
            base.Init();
            Mesh.Scale = new TGCVector3(10.5f, 10.5f, 10.5f);
        }


    }
}
