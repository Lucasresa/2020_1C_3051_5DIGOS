using System.Collections.Generic;
using TGC.Group.Model.Objects;

namespace TGC.Group.Model
{
    internal class GameEventsManager
    {
        private struct Constants
        {
            public static float TIME_BETWEEN_ATTACKS = 20;
        }

        private readonly Shark Shark;
        private readonly Character Character;
        private float timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;

        public bool SharkIsAttacking { get; private set; } = false;

        public GameEventsManager(Shark shark, Character character)
        {
            Shark = shark;
            Character = character;
        }

        public void Update(float elapsedTime, List<Fish> fishes)
        {
            if (Character.IsOutsideShip)
            {
                CheckIfSharkCanAttack(elapsedTime);
            }
            else
            {
                Shark.EndSharkAttack();
                timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
                InformFinishFromAttack();
            }
            fishes.ForEach(fish => fish.ActivateMove =  Character.IsOutsideShip);
        }

        public void InformFinishFromAttack() => SharkIsAttacking = false;

        private void CheckIfSharkCanAttack(float elapsedTime)
        {
            if (!SharkIsAttacking)
            {
                timeBetweenAttacks -= elapsedTime;
                if (timeBetweenAttacks <= 0)
                {
                    Shark.ActivateShark(this);
                    SharkIsAttacking = true;
                    timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
                }
            }
        }
    }
}
