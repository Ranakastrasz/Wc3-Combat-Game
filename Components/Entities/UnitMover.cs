using System.Numerics;

using AssertUtils;

using AStar;

using Wc3_Combat_Game.Components.Entitys;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Components.Entities
{
    public class UnitMover: IMoveable
    {
        public Vector2 Velocity { get; set; }

        public UnitMover(Vector2? velocity = null)
        {
            if(velocity != null)
            {
                Velocity = velocity.Value;
            }
            else
            {
                Velocity = Vector2.Zero;
            }
        }

        public void UpdateMovement(Entity owner, float deltaTime, IBoardContext context)
        {
            MobileEntity? entity = owner as MobileEntity;
            ICollidable? collider = entity?.Collider;
            Vector2 newPosition = owner.Position + Velocity * deltaTime;
            if(collider == null)
            {
                owner.Position = newPosition;
                return;
            }
            else
            {
                Map? map = context.Map;
                AssertUtil.NotNull(map);

                // Try full movement first.
                if(collider.HasClearPathTo(owner, newPosition, context))
                { 
                    owner.Position = newPosition;
                    return;
                }
                
                // Try X-only
                Vector2 xOnly = owner.Position + new Vector2(Velocity.X * deltaTime, 0);
                bool xOk = collider.HasClearPathTo(owner,xOnly, context);
                
                // Try Y-only
                Vector2 yOnly = owner.Position + new Vector2(0, Velocity.Y * deltaTime);
                bool yOk = collider.HasClearPathTo(owner,yOnly, context);

                if(xOk)
                    owner.Position += new Vector2(Velocity.X * deltaTime, 0);
                else if(yOk)
                    owner.Position += new Vector2(0, Velocity.Y * deltaTime);
                else
                {
                }

                collider.OnTerrainCollision(owner, context);
            }
        }
    }
}