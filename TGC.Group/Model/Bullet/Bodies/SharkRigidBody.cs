using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Input;
using TGC.Group.Utils;

namespace TGC.Group.Model.Bullet.Bodies
{
    class SharkRigidBody : RigidBodies
    {
        public override void Init()
        {

        }

        public override void Update(TgcD3dInput input)
        {
            throw new NotImplementedException();
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
