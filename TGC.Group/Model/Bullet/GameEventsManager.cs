using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Bullet.Bodies;

namespace TGC.Group.Model.Bullet
{
    class GameEventsManager
    {

        private struct Constants
        {
            public static float TIME_BETWEEN_ATTACKS = 20;
        }
        private SharkRigidBody Shark;
        private CharacterRigidBody Character;
        private float timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
        private bool atackBegan = false;

        public GameEventsManager(SharkRigidBody shark, CharacterRigidBody character)
        {
            Shark = shark;
            Character = character;
        }

        public void Update(float elapsedTime)
        {
            if (Character.IsOutside && !atackBegan)
            {
                timeBetweenAttacks -= elapsedTime;
                if (timeBetweenAttacks <= 0)
                {
                    Shark.ActivateShark(this);
                    atackBegan = true;
                    timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
                }
            }

        }

        public void InformFinishFromAttack()
        {
            atackBegan = false;
        }

    }
}
