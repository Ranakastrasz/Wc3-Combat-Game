using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototypes;
using Wc3_Combat_Game.Util;


namespace Wc3_Combat_Game.Effects
{
    internal class EffectProjectile : Effect
    {
        public ProjectilePrototype Prototype;
        public EntityManager<Projectile> ProjectileBucket;

        public EffectProjectile(ProjectilePrototype prototype, EntityManager<Projectile> projectileBucket)
        {
            Prototype = prototype;
            ProjectileBucket = projectileBucket;
        }

        protected override void Execute(IEntity Source, float currentTime)
        {
            // Maybe launch from source facing?

        }
        public override void ApplyToEntity(IEntity Caster, IEntity Emitter, IEntity Target, float currentTime)
        {
            ApplyToPoint(Caster,Emitter,Target.Position,currentTime);
        }

        public override void ApplyToPoint(IEntity Caster, IEntity Emitter, Vector2 TargetPoint, float currentTime)
        {
            //Projectile projectile = new Projectile(Caster.Position, Prototype);
            Vector2 velocity =  GeometryUtils.NormalizeAndScale(TargetPoint - Caster.Position, Prototype.Speed);
            Projectile projectile = new Projectile(
                Caster,
                Caster.Position,
                new Vector2(Prototype.Size, Prototype.Size),
                Prototype.FillBrush,
                velocity,
                Prototype.Lifespan,
                Prototype.ImpactEffect);
            projectile.Team = Caster.Team;
            ProjectileBucket.Add(projectile);
        }

    }
}
