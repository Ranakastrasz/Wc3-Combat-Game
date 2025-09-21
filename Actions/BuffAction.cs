using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Buffs;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.EntityTypes;

namespace Wc3_Combat_Game.Actions
{
    internal readonly struct BuffAction: IGameplayAction
    {
        public readonly IBuffable.BuffType Type;
        public readonly float Modifier;
        public readonly float Duration;

        internal BuffAction(IBuffable.BuffType type, float modifier, float duration)
        {
            AssertUtil.NotLess(duration, 0, true);
            Type = type;
            Modifier = modifier;
            Duration = duration;
        }

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            if(Target is Unit unit)
            {
                unit.ApplyBuff(Type, Duration, Modifier, context);
            }
        }
        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
        {
            // DoNothing.
            // Damage requires a target, or an AOE Effect to find targets.
        }

    }
}