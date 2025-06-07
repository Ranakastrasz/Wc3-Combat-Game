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
    internal class ActionProjectile : Action
    {
        public PrototypeProjectile Prototype;

        public ActionProjectile(PrototypeProjectile prototype)
        {
            Prototype = prototype;
        }

        protected override void Execute(Entities.Entity? Source, IBoardContext context)
        {
            // Maybe launch from source facing?

        }
        public override void ApplyToEntity(Entities.Entity? Caster, Entities.Entity? Emitter, Entities.Entity Target, IBoardContext context)
        {
            ApplyToPoint(Caster,Emitter,Target.Position, context);
        }

        public override void ApplyToPoint(Entities.Entity? Caster, Entities.Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
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
