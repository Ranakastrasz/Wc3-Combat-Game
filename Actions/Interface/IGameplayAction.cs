using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.Actions.Interface
{

    public interface IGameplayAction
    {
        // Caster is credited with damage or w.e.
        // Emitter is physical location of the effect's source, i.e. exploding projectile.
        // Target is the entity that is affected by the action, i.e. the unit that takes damage.

        // May need to somehow pass a team though instead at some point instead, for sourceless, but team-owned effects.

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context);
        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldVector2 TargetPoint, IBoardContext context);
    }
}
