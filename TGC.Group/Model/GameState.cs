using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model
{
    class GameState
    {
        public Action Update { get; set; }
        public Action Render { get; set; }
    }
}
