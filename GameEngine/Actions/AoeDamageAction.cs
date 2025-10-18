using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Units;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Util.UnitConversion;

using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;


namespace Wc3_Combat_Game.GameEngine.Actions
{
    public record AoeDamageAction: IGameplayAction
    {
        public string ID { get; init; }
        public float DamageMax { get; init; }
        public float DamageMin { get; init; }
        public WorldLength Radius { get; init; }
        public bool OnTargetOnly { get; init; }

        public AoeDamageAction(string id, float damageMax, float damageMin, WorldLength radius, bool onTargetOnly = false)
        {
            ID = id;
            DamageMax = damageMax;
            DamageMin = damageMin;
            Radius = radius;
            OnTargetOnly = onTargetOnly;
        }

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            ExecuteOnPoint(Caster, Emitter, Target.Position.World(), context);
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldPoint TargetPoint, IBoardContext context)
        {
            if(Caster == null) return; // May need to somehow pass a team though instead at some point instead for the baseclass, for sourceless, but team-owned effects.
            context.Entities.ForEach(targetCandidate =>
            {
                if(targetCandidate is Unit unit)
                {
                    if(Caster.Team.IsHostileTo(targetCandidate.Team))
                    {
                        WorldPoint origin = OnTargetOnly ? TargetPoint : Emitter?.Position.World() ?? TargetPoint;
                        WorldLength distance = GeometryUtils.DistanceTo(origin, targetCandidate.Position.World());
                        if(distance < Radius)
                        {
                            float damage = (Radius - distance) / Radius; // 0..1 scale
                            damage = (1.0f - damage) * DamageMin + damage * DamageMax; // real scale
                            unit.Damage(damage, context);
                        }
                    }
                }
            });
        }
    }
}