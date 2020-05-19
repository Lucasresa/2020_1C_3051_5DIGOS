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
            public static float TIME_BETWEEN_ATTACKS = 10;
        }
        private SharkRigidBody Shark;
        private CharacterRigidBody Character;
        private float timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
        private bool isAttacking = false;
        public bool SharkIsAttacking { get { return isAttacking; } }
        public GameEventsManager(SharkRigidBody shark, CharacterRigidBody character)
        {
            Shark = shark;
            Character = character;
        }

        public void Update(float elapsedTime, List<FishMesh> fishes)
        {
            if (Character.IsOutside)
            {
                fishes.ForEach(fish => fish.ActivateMove = true);
                CheckIfSharkCanAttack(elapsedTime);
            }
            else
            {
                Shark.EndSharkAttack();
                timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
                InformFinishFromAttack();
                fishes.ForEach(fish => fish.ActivateMove = false);
            }


        }

        public void InformFinishFromAttack()
        {
            isAttacking = false;
        }

        private void CheckIfSharkCanAttack(float elapsedTime)
        {
            if (!isAttacking)
            {
                timeBetweenAttacks -= elapsedTime;
                if (timeBetweenAttacks <= 0)
                {
                    Shark.ActivateShark(this);
                    isAttacking = true;
                    timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
                }
            }
        }

    }
}
