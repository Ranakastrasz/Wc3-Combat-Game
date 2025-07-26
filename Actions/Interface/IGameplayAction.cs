using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Actions.Interface
{

    public interface IGameplayAction
    {
        // Caster is credited with damage or w.e.
        // Emitter is physical location of the effect's source, i.e. exploding projectile.
        // Target is the entity that is affected by the action, i.e. the unit that takes damage.
        
        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context);
        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context);
    }
}
