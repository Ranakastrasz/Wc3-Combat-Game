using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Units;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.GameEngine.Actions
{
    public record DamageAction: IGameplayAction
    {
        public string ID { get; init; }
        public float Damage { get; init; }

        public DamageAction(string id, float damage)
        {
            ID = id;
            Damage = damage;
        }


        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            if(Target is Unit unit)
            {
                unit.Damage(Damage, context);
            }
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldPoint TargetPoint, IBoardContext context)
        {
            // DoNothing.
            // Damage requires a target, or an AOE Effect to find targets.
        }

    }
}
