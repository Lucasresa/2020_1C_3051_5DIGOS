using TGC.Core.Mathematica;
using TGC.Group.Model.Objects;

namespace TGC.Group.Model.Status
{
    class SharkStatus
    {
        public struct Constants
        {
            public static int LIFE_MAX = 250;
            public static int LIFE_MIN = 0;
            public static float LIFE_REDUCE_STEP = -0.5f;
            public static float DAMAGE_RECEIVED = 50f;
        }

        private float DamageAcumulated = 0;
        private Shark Shark { get; set; }

        public float Life { get; set; } = Constants.LIFE_MAX;
        public bool IsDead { get { return Life == 0; } }

        public SharkStatus(Shark shark)
        {
            Shark = shark;
        }

        public float GetLifeMax()
        {
            return Constants.LIFE_MAX;
        }

        public void Reset()
        {
            Life = Constants.LIFE_MAX;
            DamageAcumulated = 0;
        }

        public void Update()
        {
            if (IsDead)
                return;

            if (Shark.DamageReceived)
            {
                TakeDamage();
                Shark.DamageReceived = false;
            }

            if (DamageAcumulated > 0)
            {
                UpdateLife(Constants.LIFE_REDUCE_STEP);
                DamageAcumulated += Constants.LIFE_REDUCE_STEP;
            }
        }

        private void UpdateLife(float value)
        {
            Life += value;
            Life = FastMath.Clamp(Life, Constants.LIFE_MIN, Constants.LIFE_MAX);
        }

        private void TakeDamage()
        {
            DamageAcumulated = Constants.DAMAGE_RECEIVED;
        }
    }
}
