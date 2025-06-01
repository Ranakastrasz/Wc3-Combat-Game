using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Interface
{
    class IBasicWeapon : IWeapon
    {
        private float _cooldown;
        private float _lastShotTime = float.NegativeInfinity;

        private float _attackRange;
        private Effect _effect;

        public IBasicWeapon(Effect effect, float cooldown, float range)
        {
            _effect = effect;
            _cooldown = cooldown;
            _attackRange = range;
        }

        public bool TryShoot(Unit unit, Vector2 target, float currentTime)
        {
            if (currentTime < _lastShotTime + _cooldown)
                return false;

            _effect.ApplyToPoint(unit, unit, target, currentTime);

            _lastShotTime = currentTime;
            return true;
        }

        public float GetCooldown() => _cooldown;
        public float GetTimeSinceLastShot(float currentTime) => currentTime - _lastShotTime;

        public float GetAttackRange() => _attackRange;

    }
}
