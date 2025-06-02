using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Interface.Weapons
{
    class IWeaponBasic : IWeapon
    {
        private float _cooldown;
        private float _lastShotTime = float.NegativeInfinity;

        private float _attackRange;
        private Effect? _CastEffect;

        PrototypeWeapon _prototype;


        public IWeaponBasic(PrototypeWeaponBasic prototype)
        {
            _prototype = prototype;
            _CastEffect = prototype.CastEffect;
            _cooldown = prototype.Cooldown;
            _attackRange = prototype.CastRange;
        }

        public bool TryShootPoint(Unit unit, Vector2 target, IBoardContext context)
        {
            if (!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _CastEffect?.ApplyToPoint(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public bool TryShootEntity(Unit unit, IEntity target, IBoardContext context)
        {
            if (!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _CastEffect?.ApplyToEntity(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public float GetCooldown() => _cooldown;
        public float GetTimeSinceLastShot(IDrawContext context) => context.CurrentTime - _lastShotTime;

        public float GetAttackRange() => _attackRange;

        public PrototypeWeapon? GetPrototype()
        {
            return _prototype;
        }
    }
}
