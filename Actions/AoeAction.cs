using System.Numerics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;


namespace Wc3_Combat_Game.Actions
{
    public record AoeAction: IGameplayAction
    {
        public float Radius { get; init; }
        public IGameplayAction Action { get; init; }

        public AoeAction(float radius, IGameplayAction action)
        {
            Radius = radius;
            Action = action;
        }

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            ExecuteOnPoint(Caster,Emitter,Target.Position,context);
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
        {
            if(Caster == null) return; // May need to somehow pass a team though instead at some point.
            context.Entities.ForEach(e =>
            {
                if(Caster.Team.IsHostileTo(e.Team))
                {
                    if(Caster.DistanceSquaredTo(e) < Radius*Radius)
                    {
                        ExecuteOnEntity(Caster, Emitter, e, context);
                    }
                }
            });
        }
    }
}