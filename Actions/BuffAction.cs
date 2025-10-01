using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Units;
using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.Actions
{
    internal record BuffAction: IGameplayAction
    {
        public IBuffable.BuffType Type { get; init; }
        public float Factor { get; init; }
        public float Duration { get; init; }

        internal BuffAction(IBuffable.BuffType type, float factor, float duration)
        {
            AssertUtil.NotLess(duration, 0, true);
            Type = type;
            Factor = factor;
            Duration = duration;
        }

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            if(Target is Unit unit)
            {
                unit.ApplyBuff(Type, Duration, Factor, context);
            }
        }
        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldVector2 TargetPoint, IBoardContext context)
        {
            // DoNothing.
            // Damage requires a target, or an AOE Effect to find targets.
        }

    }
}