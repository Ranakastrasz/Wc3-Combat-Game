using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities.Components.Movement
{
    public class UnitMover: IMoveable
    {
        public Vector2 Velocity { get; set; }

        // Tempory, psudobuff crap.

        public float SlowExpires { get; set; } = float.NegativeInfinity;

        public UnitMover(Vector2? velocity = null)
        {
            Velocity = velocity ?? Vector2.Zero;
        }

        public void Update(Entity owner, float deltaTime, IBoardContext context)
        {
            ICollidable? collider = owner.Collider;


            Vector2 deltaMove = Velocity * deltaTime;

            if(!TimeUtils.HasElapsed(context.CurrentTime, SlowExpires, 0f))
            {
                deltaMove *= 0.75f;
            }

            Vector2 newPosition = owner.Position + deltaMove;
            if(collider == null)
            {
                owner.Position = newPosition;
                return;
            }
            else
            {
                Map map = context.Map;
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
        private Vector2 StepUntilBlocked(Entity owner, ICollidable collider, Vector2 delta, Map map, IBoardContext context)
        {
            const int steps = 10;
            Vector2 start = owner.Position;
            Vector2 step = delta / steps;
            Vector2 pos = start;

            for(int i = 1; i <= steps; i++)
            {
                Vector2 test = start + step * i;
                if(!collider.HasClearPathTo(owner, test, context)) // use cheap collision check here
                    pos = test;
                else
                    break;
            }

            return pos;
        }
    }
}