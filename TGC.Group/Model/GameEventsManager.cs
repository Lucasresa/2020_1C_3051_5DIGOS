using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Group.Model.Objects;
using static TGC.Group.Model.Objects.Fish;

namespace TGC.Group.Model
{
    class GameEventsManager
    {
        private struct Constants
        {
            public static float TIME_BETWEEN_ATTACKS = 10;
        }

        private Shark Shark;
        private Character Character;
        private float timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
        private bool isAttacking = false;
        public bool SharkIsAttacking { get { return isAttacking; } }
        
        public GameEventsManager(Shark shark, Character character)
        {
            Shark = shark;
            Character = character;
        }

        public void Update(float elapsedTime, Fish fish)
        {
            if (Character.IsOutsideShip)
            {
                fish.ActivateMove = true;
                CheckIfSharkCanAttack(elapsedTime);
            }
            else
            {
                Shark.EndSharkAttack();
                timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
                InformFinishFromAttack();
                fish.ActivateMove = false;
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
