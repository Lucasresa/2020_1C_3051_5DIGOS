using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;

namespace TGC.Group.Model.Bullet.Bodies
{
    class ShipRigidBody : RigidBodies
    {
        public override void Init()
        {

        }

        public override void Render()
        {
            throw new NotImplementedException();
        }

        public override void Dispose()
        {
            RigidBody.Dispose();
        }
    }
}
