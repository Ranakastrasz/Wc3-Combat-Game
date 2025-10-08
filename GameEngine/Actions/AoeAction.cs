using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Units;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Util.UnitConversion;


namespace Wc3_Combat_Game.GameEngine.Actions
{
    public record AoeAction: IGameplayAction
    {
        public IGameplayAction? Action { get; init; }
        public WorldLength Radius { get; init; }
        public bool OnTargetOnly { get; init; }

        public AoeAction(WorldLength radius, IGameplayAction? action, bool onTargetOnly)
        {
            Radius = radius;
            Action = action;
            OnTargetOnly = onTargetOnly;
        }

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            ExecuteOnPoint(Caster,Emitter,Target.Position.World(),context);
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldPoint TargetPoint, IBoardContext context)
        {
            if(Action == null) return;
            if(Caster == null) return; // May need to somehow pass a team though instead at some point.
            context.Entities.ForEach(e =>
            {
                if(e is Unit unit)
                {
                    if(Caster.Team.IsHostileTo(e.Team))
                    {
                        WorldPoint origin = OnTargetOnly ? TargetPoint : Emitter?.Position.World() ?? TargetPoint;
                        WorldLength distance = GeometryUtils.DistanceTo(origin, e.Position.World());
                        if(distance < Radius)
                        {
                            ExecuteOnEntity(Caster, Emitter, e, context);
                        }
                    }
                }
            });
        }
    }
}