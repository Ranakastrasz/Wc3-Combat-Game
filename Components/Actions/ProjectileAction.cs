﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Util;


namespace Wc3_Combat_Game.Components.Actions
{
    internal class ProjectileAction : IGameplayAction
    {
        public ProjectilePrototype Prototype;

        public ProjectileAction(ProjectilePrototype prototype)
        {
            Prototype = prototype;
        }

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            ExecuteOnPoint(Caster,Emitter,Target.Position, context);
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, Vector2 TargetPoint, IBoardContext context)
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
