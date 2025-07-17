using System.Numerics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities.Components.Abilities
{
    class TargetedAbility: IAbility
    {
        protected float _cooldown;
        protected float _lastShotTime = float.NegativeInfinity;

        protected float _range;
        protected float _rangeSqr;
        public float Cooldown => _cooldown;
        public float LastShotTime => _lastShotTime;
        public float UseRange => _range;
        public float UseRangeSqr => _rangeSqr;

        protected IGameplayAction? _CastEffect;

        protected readonly AbilityPrototype _prototype;



        public TargetedAbility(TargetedAbilityPrototype prototype)
        {
            _prototype = prototype;
            _CastEffect = prototype.CastEffect;
            _cooldown = prototype.Cooldown;
            _range = prototype.CastRange;
            _rangeSqr = _range * _range;
        }

        public bool TryTargetPoint(Unit unit, Vector2 target, IBoardContext context)
        {
            if(!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            if(_prototype is TargetedAbilityPrototype basic) // This is hidious, and needs to be encapsulated better.
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

        public bool TryTargetEntity(Unit unit, Entity target, IBoardContext context)
        {
            if(!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            _CastEffect?.ExecuteOnEntity(unit, unit, target, context);

            _lastShotTime = context.CurrentTime;
            return true;
        }

        public float GetTimeSinceLastUse(IContext context) => context.CurrentTime - _lastShotTime;


        public AbilityPrototype? GetPrototype()
        {
            return _prototype;
        }
    }
}
