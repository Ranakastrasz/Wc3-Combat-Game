using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Interface.Weapons;

namespace Wc3_Combat_Game.Prototype
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

        public bool TryShootPoint(Unit unit, Vector2 target, BoardContext context) => false;

        public float GetCooldown() => float.PositiveInfinity;

        public float GetTimeSinceLastShot(BoardContext context) => float.PositiveInfinity;
        public float GetAttackRange() => 0f;
    }
}
