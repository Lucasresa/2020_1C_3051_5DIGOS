using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.Mathematica;
using TGC.Group.Model.Objects;

namespace TGC.Group.Model.Status
{
    class CharacterStatus
    {
        public struct Constants
        {
            public static int LIFE_MAX = 100;
            public static int LIFE_MIN = 0;
            public static int OXYGEN_MAX = 100;
            public static int OXYGEN_MIN = 0;
            public static float LIFE_REDUCE_STEP = -0.3f;
            public static float LIFE_INCREMENT_STEP = 0.01f;
            public static float OXYGEN_INCREMENT_STEP = 1f;
            public static float OXYGEN_REDUCE_STEP = -0.05f;
            public static float OXYGEN_REDUCE_STEP_WHIT_DIVING_HELMET = -0.0025f;
            public static float DAMAGE_RECEIVED = 30f;
        }

        private Character Character { get; set; }
        private bool CanBreathe => (Character.IsInsideShip || Character.IsOutOfWater) && !IsDead; 
        private float DamageAcumulated = 0;
        public bool ActiveAlarmForDamageReceived { get; set; }

        public float Life { get; set; } = Constants.LIFE_MAX;
        public float Oxygen { get; set; } = Constants.OXYGEN_MAX;
        public bool IsDead => Oxygen == 0 || Life == 0;
        public bool HasDivingHelmet { get; set; }
        public bool DamageReceived { get; set; }
        public bool ActiveRenderAlarm => Life < 20 || Oxygen < 30 || ActiveAlarmForDamageReceived;

        public CharacterStatus(Character character) => Character = character;

        public float GetLifeMax() => Constants.LIFE_MAX;

        public float GetOxygenMax() => Constants.OXYGEN_MAX;

        private void RecoverLife() => UpdateLife(Constants.LIFE_INCREMENT_STEP);
        
        public void Reset()
        {
            Life = Constants.LIFE_MAX;
            Oxygen = Constants.OXYGEN_MAX;
            DamageAcumulated = 0;
        }

        public void Update()
        {
            if (DamageReceived)
            {
                TakeDamage();
                DamageReceived = !DamageReceived;
                ActiveAlarmForDamageReceived = true;
            }

            if (DamageAcumulated > 0)
            {
                UpdateLife(Constants.LIFE_REDUCE_STEP);
                DamageAcumulated += Constants.LIFE_REDUCE_STEP;
            }

            if (Character.IsInsideShip)
                RecoverLife();

            if (CanBreathe)
                UpdateOxygen(Constants.OXYGEN_INCREMENT_STEP);
            else
                if (HasDivingHelmet)
                    UpdateOxygen(Constants.OXYGEN_REDUCE_STEP_WHIT_DIVING_HELMET);
                else
                    UpdateOxygen(Constants.OXYGEN_REDUCE_STEP);
        }

        private void UpdateLife(float value) => Life = FastMath.Clamp(Life + value, Constants.LIFE_MIN, Constants.LIFE_MAX);

        private void UpdateOxygen(float value) => Oxygen = FastMath.Clamp(Oxygen + value, Constants.OXYGEN_MIN, Constants.OXYGEN_MAX);

        private void TakeDamage() => DamageAcumulated = Constants.DAMAGE_RECEIVED;

        public void Respawn()
        {
            Reset();
            Character.Respawn();
        }
    }
}
