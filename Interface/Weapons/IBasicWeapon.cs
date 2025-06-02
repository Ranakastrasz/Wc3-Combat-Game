using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Interface.Weapons
{
    class IBasicWeapon : IWeapon
    {
        private float _cooldown;
        private float _lastShotTime = float.NegativeInfinity;

        private float _attackRange;
        private Effect? _CastEffect;
        //private BasicWeaponPrototype _prototype;

        public IBasicWeapon(Effect effect, float cooldown, float range)
        {
            _CastEffect = effect;
            _cooldown = cooldown;
            _attackRange = range;
        }
        public IBasicWeapon(BasicWeaponPrototype prototype)
        {
            //_prototype = prototype;
            _CastEffect = prototype.CastEffect;
            _cooldown = prototype.Cooldown;
            _attackRange = prototype.CastRange;
        }

        public bool TryShootPoint(Unit unit, Vector2 target, BoardContext context)
        {
            if (!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _CastEffect?.ApplyToPoint(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public bool TryShootEntity(Unit unit, IEntity target, BoardContext context)
        {
            if (!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _CastEffect?.ApplyToEntity(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public float GetCooldown() => _cooldown;
        public float GetTimeSinceLastShot(BoardContext context) => context.CurrentTime - _lastShotTime;

        public float GetAttackRange() => _attackRange;

    }
}
