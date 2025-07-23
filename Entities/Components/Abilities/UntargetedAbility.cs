using System.Numerics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities.Components.Abilities
{
    class UntargetedAbility: IAbility
    {
        protected float _cooldown;
        protected float _lastShotTime = float.NegativeInfinity;
        public float Cooldown => _cooldown;
        public float LastShotTime => _lastShotTime;

        protected IGameplayAction? _CastEffect;

        protected readonly AbilityPrototype _prototype;



        public UntargetedAbility(TargetedAbilityPrototype prototype)
        {
            _prototype = prototype;
            _CastEffect = prototype.CastEffect;
            _cooldown = prototype.Cooldown;
        }

        public bool TryTargetPoint(Unit unit, Vector2 target, IBoardContext context)
        {
            if(!TimeUtils.HasElapsed(context.CurrentTime, _lastShotTime, _cooldown))
                return false;

            if(_prototype is TargetedAbilityPrototype basic)
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


        public float GetTimeSinceLastUse(IContext context) => context.CurrentTime - _lastShotTime;


        public AbilityPrototype? GetPrototype()
        {
            return _prototype;
        }

        public bool OnCooldown(float currentTime)
        {
            return !TimeUtils.HasElapsed(currentTime, _lastShotTime, _cooldown);
        }
    }
}
