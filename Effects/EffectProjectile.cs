using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Util;


namespace Wc3_Combat_Game.Effects
{
    internal class EffectProjectile : Effect
    {
        public PrototypeProjectile Prototype;

        public EffectProjectile(PrototypeProjectile prototype)
        {
            Prototype = prototype;
        }

        protected override void Execute(IEntity? Source, BoardContext context)
        {
            // Maybe launch from source facing?

        }
        public override void ApplyToEntity(IEntity? Caster, IEntity? Emitter, IEntity Target, BoardContext context)
        {
            ApplyToPoint(Caster,Emitter,Target.Position, context);
        }

        public override void ApplyToPoint(IEntity? Caster, IEntity? Emitter, Vector2 TargetPoint, BoardContext context)
        {
            //Projectile projectile = new Projectile(Caster.Position, Prototype);
            // Hardcode requirements for now
            if (Caster == null || Emitter == null) return;
                Vector2 directionToTarget = TargetPoint - Caster.Position;
            Projectile projectile = new
                (Prototype, Caster, Emitter.Position, directionToTarget)
            {
                Team = Caster.Team
            };
            context.AddProjectile(projectile);
        }

    }
}
