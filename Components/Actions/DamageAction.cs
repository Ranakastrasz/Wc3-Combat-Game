using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Components.Actions
{
    internal class DamageAction : IGameplayAction
    {

        public float Damage;

        internal DamageAction(float damage)
        {
            Damage = damage;
        }


        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            if (Target is Unit unit)
            {
                unit.Damage(Damage, context);
            }
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
        {
            // DoNothing.
            // Damage requires a target, or an AOE Effect to find targets.
        }

    }
}
