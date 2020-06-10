using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TGC.Group.Model.Objects;
using TGC.Group.Model.Status;

namespace TGC.Group.Model.Callbacks
{
    class SharkAttackCallback : ContactResultCallback
    {
        private struct Constants
        {
            public static float DAMAGE_TO_CHARACTER = 30f;
        }

        public Shark Shark { get; }
        public CharacterStatus CharacterStatus { get; }

        public SharkAttackCallback(Shark shark, CharacterStatus characterStatus)
        {
            Shark = shark;
            CharacterStatus = characterStatus;
        }

        public override float AddSingleResult(ManifoldPoint cp, CollisionObjectWrapper colObj0Wrap, int partId0, int index0, CollisionObjectWrapper colObj1Wrap, int partId1, int index1)
        {
            if (Shark.CharacterOnSight)
            {
                CharacterStatus.DamageReceived = Constants.DAMAGE_TO_CHARACTER;
                Shark.ChangeSharkWay();
            }
            return 0;
        }

        public override bool NeedsCollision(BroadphaseProxy proxy)
        {
            if (base.NeedsCollision(proxy))
                return Shark.Body.CheckCollideWithOverride(proxy.ClientObject as CollisionObject);
            else
                return false;
        }
    }
}
