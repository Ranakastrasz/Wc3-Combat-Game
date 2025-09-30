using System.Numerics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Units;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities.Units.Abilities
{
    public class Ability
    {
        protected float _cooldown;
        protected float _lastShotTime = float.NegativeInfinity;

        protected float _range;
        protected float _rangeSqr;
        public float Cooldown => _cooldown;
        public float LastShotTime => _lastShotTime;
        public float UseRange => _range;
        public float UseRangeSqr => _rangeSqr;

        protected IGameplayAction? _targetEffect;
        protected IGameplayAction? _casterEffect;

        protected readonly AbilityPrototype _prototype;



        public Ability(AbilityPrototype prototype)
        {
            _prototype = prototype;
            _targetEffect = prototype.TargetEffect;
            _casterEffect = prototype.CasterEffect;
            _cooldown = prototype.Cooldown;
            _range = prototype.CastRange;
            _rangeSqr = _range * _range;
        }

        public bool TryTargetPoint(Unit unit, Vector2 target, IBoardContext context)
        {
            if(!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            if(_prototype is AbilityPrototype basic) // This is hidious, and needs to be encapsulated better.
            {
                if(basic.ManaCost > 0)
                {
                    if(unit.Mana < basic.ManaCost)
                        return false; // Not enough mana to cast.
                    unit.Mana -= basic.ManaCost; // Deduct mana cost.
                }
            }

            _targetEffect?.ExecuteOnPoint(unit, unit, target, context);

            _casterEffect?.ExecuteOnEntity(unit, unit, unit, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public bool TryTargetEntity(Unit unit, Entity target, IBoardContext context)
        {
            if(!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _targetEffect?.ExecuteOnEntity(unit, unit, target, context);

            _casterEffect?.ExecuteOnEntity(unit, unit, unit, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public float GetTimeSinceLastUse(IContext context) => context.CurrentTime - _lastShotTime;


        public AbilityPrototype? GetPrototype()
        {
            return _prototype;
        }

        public bool OnCooldown(float currentTime)
        {
            return !TimeUtils.HasElapsed(currentTime, _lastShotTime, _cooldown);
        }

        internal void TryTarget(Unit unit, Unit? targetUnit, Vector2 targetPosition, IBoardContext context)
        {
            if(targetUnit != null)
                TryTargetEntity(unit, targetUnit, context);
            else
                TryTargetPoint(unit, targetPosition, context);
        }
    }
}
