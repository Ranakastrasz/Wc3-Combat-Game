using System.Numerics;
using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Components.Weapons.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Components.Weapons
{
    class BasicWeapon : IWeapon
    {
        private float _cooldown;
        private float _lastShotTime = float.NegativeInfinity;

        private float _attackRange;
        private IGameplayAction? _CastEffect;

        WeaponPrototype _prototype;


        public BasicWeapon(WeaponPrototypeBasic prototype)
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

            _CastEffect?.ExecuteOnPoint(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public bool TryShootEntity(Unit unit, Entity target, IBoardContext context)
        {
            if (!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _CastEffect?.ExecuteOnEntity(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public float GetCooldown() => _cooldown;
        public float GetTimeSinceLastShot(IDrawContext context) => context.CurrentTime - _lastShotTime;

        public float GetAttackRange() => _attackRange;

        public WeaponPrototype? GetPrototype()
        {
            return _prototype;
        }
    }
}
