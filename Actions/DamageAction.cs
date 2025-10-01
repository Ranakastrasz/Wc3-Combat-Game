using System.Numerics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Units;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.Actions
{
    internal record DamageAction: IGameplayAction
    {

        public float Damage { get; init; }

        internal DamageAction(float damage)
        {
            Damage = damage;
        }


        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            if(Target is Unit unit)
            {
                unit.Damage(Damage, context);
            }
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldVector TargetPoint, IBoardContext context)
        {
            // DoNothing.
            // Damage requires a target, or an AOE Effect to find targets.
        }

    }
}
