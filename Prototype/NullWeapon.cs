using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Prototypes
{
    internal class NullWeapon : IWeapon
    {
        public static readonly NullWeapon INSTANCE = new NullWeapon();

        public float AttackRange
        {
            get => 0f;
            set { } // no-op
        }

        private NullWeapon() { }

        public bool TryShoot(Unit unit, Vector2 target, float currentTime) => false;

        public float GetCooldown() => float.PositiveInfinity;

        public float GetTimeSinceLastShot(float currentTime) => float.PositiveInfinity;
        public float GetAttackRange() => 0f;
    }
}
