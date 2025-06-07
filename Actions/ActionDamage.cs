using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Effects
{
    internal class ActionDamage : Action
    {
        public float Damage;

        internal ActionDamage(float damage)
        {
            Damage = damage;
        }

        protected override void Execute(Entities.Entity? Source, IBoardContext context)
        {

        }
        public override void ApplyToEntity(Entities.Entity? Caster, Entities.Entity? Emitter, Entities.Entity Target, IBoardContext context)
        {
            if (Target is Unit unit)
            {
                unit.Damage(Damage, context);
            }
        }

        public override void ApplyToPoint(Entities.Entity? Caster, Entities.Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
        {
            // DoNothing.
            // Damage requires a target, or an AOE Effect to find targets.
        }

    }
}
