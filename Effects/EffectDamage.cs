using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Effects
{
    internal class EffectDamage : Effect
    {
        public float Damage;

        internal EffectDamage(float damage)
        {
            Damage = damage;
        }

        protected override void Execute(IEntity Source, float currentTime)
        {

        }
        public override void ApplyToEntity(IEntity Caster, IEntity Emitter, IEntity Target, float currentTime)
        {
            if (Target is Unit unit)
            {
                unit.Damage(Damage, currentTime);
            }
        }

        public override void ApplyToPoint(IEntity Caster, IEntity Emitter, Vector2 TargetPoint, float currentTime)
        {
            // DoNothing.
        }

    }
}
