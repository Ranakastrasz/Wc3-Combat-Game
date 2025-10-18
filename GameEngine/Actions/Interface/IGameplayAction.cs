using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.GameEngine.Actions.Interface
{
    // Caster is credited with damage or w.e.
    // Emitter is physical location of the effect's source, i.e. exploding projectile.
    // Target is the entity that is affected by the action, i.e. the unit that takes damage.

    // May need to somehow pass a team though instead at some point instead, for sourceless, but team-owned effects.
    // If and only if I actually need that at some point though.


    public interface IGameplayAction
    {
        public string ID { get; }
        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context);
        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldPoint TargetPoint, IBoardContext context);
    }
}
