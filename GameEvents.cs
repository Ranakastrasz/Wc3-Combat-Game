using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Interface;

namespace Wc3_Combat_Game.Events
{
    public abstract class GameEvent
    {
        public IBoardContext Context { get; }
        public Entity? Caster { get; }

        protected GameEvent(IBoardContext context, Entity? caster)
        {
            Context = context;
            Caster = caster;
        }
    }

    public class DamageDealtEvent: GameEvent
    {
        public Entity Target { get; }
        public float DamageAmount { get; }
        public Entity? Emitter { get; }

        public DamageDealtEvent(Entity? caster, Entity? emitter, Entity target, float damageAmount, IBoardContext context)
            : base(context, caster)
        {
            Target = target;
            DamageAmount = damageAmount;
            Emitter = emitter;
        }
    }

    public class AbilityUsedEvent: GameEvent
    {
        public Unit Unit { get; }
        public IAbility Ability { get; }
        public Entity? TargetEntity { get; }
        public Vector2? TargetPoint { get; }

        public AbilityUsedEvent(Unit unit, IAbility ability, IBoardContext context, Entity? targetEntity = null, Vector2? targetPoint = null)
            : base(context, unit)
        {
            Unit = unit;
            Ability = ability;
            TargetEntity = targetEntity;
            TargetPoint = targetPoint;
        }
    }
}