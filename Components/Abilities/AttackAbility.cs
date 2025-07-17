using System.Numerics;

using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Components.Weapons.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Prototype.Weapons;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Components.Weapons
{
    class AttackAbility: IWeapon
    {
        protected float _cooldown;
        protected float _lastShotTime = float.NegativeInfinity;

        protected float _attackRange;
        public float Cooldown => _cooldown;
        public float LastShotTime => _lastShotTime;
        public float AttackRange => _attackRange;

        protected IGameplayAction? _CastEffect;

        protected readonly WeaponPrototype _prototype;



        public AttackAbility(WeaponPrototypeBasic prototype)
        {
            _prototype = prototype;
            _CastEffect = prototype.CastEffect;
            _cooldown = prototype.Cooldown;
            _attackRange = prototype.CastRange;
        }

        public bool TryShootPoint(Unit unit, Vector2 target, IBoardContext context)
        {
            if(!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            if(_prototype is WeaponPrototypeBasic basic) // This is hidious, and needs to be encapsulated better.
            {
                if(basic.ManaCost > 0)
                {
                    if(unit.Mana < basic.ManaCost)
                        return false; // Not enough mana to cast.
                    unit.Mana -= basic.ManaCost; // Deduct mana cost.
                }
            }

            _CastEffect?.ExecuteOnPoint(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public bool TryShootEntity(Unit unit, Entity target, IBoardContext context)
        {
            if(!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _CastEffect?.ExecuteOnEntity(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public float GetTimeSinceLastShot(IDrawContext context) => context.CurrentTime - _lastShotTime;
        public float GetTimeSinceLastShot(IBoardContext context) => context.CurrentTime - _lastShotTime;


        public WeaponPrototype? GetPrototype()
        {
            return _prototype;
        }
    }
}
