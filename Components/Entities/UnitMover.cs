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
            Velocity = velocity ?? Vector2.Zero;
        }

        public void Update(Entity owner, float deltaTime, IBoardContext context)
        {
            ICollidable? collider = owner.Collider;

            Vector2 deltaMove = Velocity * deltaTime;
            Vector2 newPosition = owner.Position + deltaMove;
            if(collider == null)
            {
                owner.Position = newPosition;
                return;
            }
            else
            {
                Map? map = context.Map;
                AssertUtil.NotNull(map);

                // Issue. This doesn't allow you to move "As far as Possible" I believe.


                // Try full movement first.
                if(collider.HasClearPathTo(owner, newPosition, context))
                { 
                    owner.Position = newPosition;
                    return;
                }
                
                // Try X-only
                Vector2 xOnly = owner.Position + new Vector2(deltaMove.X, 0);
                bool xOk = collider.HasClearPathTo(owner,xOnly, context);
                
                // Try Y-only
                Vector2 yOnly = owner.Position + new Vector2(0, deltaMove.Y);
                bool yOk = collider.HasClearPathTo(owner,yOnly, context);

                if(xOk)
                    owner.Position = xOnly;
                else if(yOk)
                    owner.Position = yOnly;
                else
                {

                }

                collider.OnTerrainCollision(owner, context);
            }
        }
    }
}