using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Movement;

namespace Wc3_Combat_Game.Actions
{
    internal class SlowAction: IGameplayAction
    {

        public float Duration;

        internal SlowAction(float duration)
        {
            AssertUtil.NotLess(duration, 0, true);
            Duration = duration;
        }


        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            if(Target is Unit unit)
            {
                unit.SlowExpires = context.CurrentTime + Duration;
            }
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
        {
            // DoNothing.
            // Damage requires a target, or an AOE Effect to find targets.
        }

    }
}