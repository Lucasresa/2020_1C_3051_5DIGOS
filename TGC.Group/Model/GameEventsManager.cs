using TGC.Group.Model.Objects;

namespace TGC.Group.Model
{
    internal class GameEventsManager
    {
        private struct Constants
        {
            public static float TIME_BETWEEN_ATTACKS = 10;
        }

        private readonly Shark Shark;
        private readonly Character Character;
        private float timeBetweenAttacks = Constants.TIME_BETWEEN_ATTACKS;
        private bool isAttacking = false;
        public bool SharkIsAttacking => isAttacking;

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

        public void InformFinishFromAttack() => isAttacking = false;

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
