using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.EntityTypes;
using Wc3_Combat_Game.Events;


namespace Wc3_Combat_Game.Core.Event
{
    namespace Wc3_Combat_Game.Core.Event
    {
        public class ProjectileImpactEvent: GameEvent
        {
            public int ProjectileId { get; }
            public int? TargetId { get; }
            public Vector2 ImpactPoint { get; }
            public int CasterId { get; }

            public ProjectileImpactEvent(int projectileId, int casterId, int? targetId, Vector2 impactPoint, IBoardContext context) :
                base(context, null)
            {
                ProjectileId = projectileId;
                CasterId = casterId;
                TargetId = targetId;
                ImpactPoint = impactPoint;
            }
        }
        public class ImpactEventHandler
        {
            private readonly IBoardContext _context;

            public ImpactEventHandler(IBoardContext context)
            {
                _context = context;
                // Subscribe this handler to the event
                _context.EventBus.Subscribe<ProjectileImpactEvent>(HandleImpact);
            }

            private void HandleImpact(ProjectileImpactEvent e)
            {
                // Find the projectile and caster by their IDs
                var projectile = _context.Entities.GetEntityByIndex(e.ProjectileId) as Projectile;
                var caster = _context.Entities.GetEntityByIndex(e.CasterId);

                // This check is important because the projectile might have already been disposed
                // before the event is processed.
                if(projectile == null || projectile.ImpactEffects.Length == 0 || caster == null)
                {
                    return;
                }

                // This is where UI/Graphic effects should occur.

                if(e.TargetId.HasValue)
                {
                    // Impacted an entity

                    // Projectile.DrawExplosion
                    // Projectile.PlayImpactSound
                    // Etc

                }
                else
                {
                    // Impacted terrain
                }
            }
        }
    }
}