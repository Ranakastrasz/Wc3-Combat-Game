using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Units;
using Wc3_Combat_Game.Entities.Units.Abilities;

namespace Wc3_Combat_Game.Core.Event
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

    // Unit Events
    // Spawn, Die, Take damage, Deal Damage Use Ability, Gain Buff, Lose Buff.
    // Projectile Events
    // Spawn, Die, Hit Target, Hit Terrain

    // As needed.

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
        public Ability Ability { get; }
        public Entity? TargetEntity { get; }
        public Vector2? TargetPoint { get; }

        public AbilityUsedEvent(Unit unit, Ability ability, IBoardContext context, Entity? targetEntity = null, Vector2? targetPoint = null)
            : base(context, unit)
        {
            Unit = unit;
            Ability = ability;
            TargetEntity = targetEntity;
            TargetPoint = targetPoint;
        }
    }
}