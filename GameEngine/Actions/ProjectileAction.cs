using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.EntityTypes;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.GameEngine.Data.Data;
using Wc3_Combat_Game.Util.UnitConversion;


namespace Wc3_Combat_Game.GameEngine.Actions
{
    internal record ProjectileAction: IGameplayAction
    {
        //public float Randomness; // 0 = perfect spread, 1 = full random. 0.5, each is randomized up to 50% from ideal position.
        public int ProjectileCount { get; init; } = 1;
        public float FullSpreadAngleDeg { get; init; } = 0f;

        public float FullSpreadAngleRad => MathF.PI * FullSpreadAngleDeg / 180f;


        public ProjectileData Prototype { get; init; }

        public ProjectileAction(ProjectileData prototype)
        {
            Prototype = prototype;
        }

        public void ExecuteOnEntity(Entity? Caster, Entity? Emitter, Entity Target, IBoardContext context)
        {
            ExecuteOnPoint(Caster, Emitter, Target.Position.World(), context);
        }

        public void ExecuteOnPoint(Entity? Caster, Entity? Emitter, WorldPoint TargetPoint, IBoardContext context)
        {
            //Projectile projectile = new Projectile(Caster.Position, Prototype);
            // Hardcode requirements for now
            if(Caster == null || Emitter == null) return;
            Vector2 directionToTarget = TargetPoint.Value - Caster.Position;
            if(ProjectileCount > 1)
            {
                float angleStep = FullSpreadAngleRad / (ProjectileCount - 1);
                float startAngle = -FullSpreadAngleRad / 2f;
                for(int i = 0; i < ProjectileCount; i++)
                {
                    float angle = startAngle + i * angleStep;
                    Vector2 rotatedDirection = Vector2.Transform(directionToTarget, Matrix3x2.CreateRotation(angle));
                    FireProjectileInDirection(Caster, Emitter, rotatedDirection, context);
                }
            }
            else
            {
                FireProjectileInDirection(Caster, Emitter, directionToTarget, context);
            }
        }

        private void FireProjectileInDirection(Entity Caster, Entity Emitter, Vector2 directionToTarget, IBoardContext context)
        {
            Projectile projectile = new
            (Prototype, Caster, Emitter.Position, directionToTarget,context)
            {
                Team = Caster.Team
            };
            context.AddProjectile(projectile);
        }

    }
}
