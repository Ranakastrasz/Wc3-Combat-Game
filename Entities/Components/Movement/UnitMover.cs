using System.Numerics;

using AssertUtils;

using AStar;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities.Components.Movement
{
    public class UnitMover: IMoveable
    {
        ICollidable? _collider;
        WorldPosition _position;
        Vector2 Position { get => _position.Position; set => _position.Position = value; }
        public Vector2 Velocity { get; set; }

        // Tempory, psudobuff crap.
        public float SlowExpires { get; set; } = float.NegativeInfinity;

        public UnitMover(WorldPosition position, ICollidable collider, Vector2? velocity = null)
        {
            _position = position;
            _collider = collider;
            Velocity = velocity ?? Vector2.Zero;
        }

        public void Update(Entity owner, float deltaTime, IBoardContext context)
        {


            Vector2 deltaMove = Velocity * deltaTime;

            if(!TimeUtils.HasElapsed(context.CurrentTime, SlowExpires, 0f))
            {
                deltaMove *= 0.75f;
            }

            Vector2 newPosition = Position + deltaMove;

            if(_collider == null)
            {
                Position = newPosition;
                return;
            }
            else
            {
                Map map = context.Map;
                AssertUtil.NotNull(map);

                // Try full movement first.
                if(_collider.HasClearPathTo(owner, newPosition, context))
                {
                    Position = newPosition;
                    return;
                }

                // If full movement is blocked, attempt to slide.
                // Try moving along the X axis only
                Vector2 xOnlyTarget = Position + new Vector2(deltaMove.X, 0);
                Vector2 attemptedXMove = StepUntilBlocked(new Vector2(deltaMove.X, 0), map, context);

                // Try moving along the Y axis only (from the original position)
                Vector2 yOnlyTarget = Position + new Vector2(0, deltaMove.Y);
                Vector2 attemptedYMove = StepUntilBlocked(new Vector2(0, deltaMove.Y), map, context);

                // Determine the best slide path
                Vector2 finalPosition = Position;

                // Option 1: Prioritize X, then Y
                if(attemptedXMove != Position) // If we can move along X
                {
                    finalPosition = attemptedXMove;
                    // Now try to add Y movement from the new X position
                    Vector2 remainingYMove = new Vector2(0, deltaMove.Y);
                    Vector2 potentialYAfterX = StepUntilBlocked(remainingYMove, map, context, finalPosition);
                    // If the Y movement from the new X position is different, combine them
                    if(potentialYAfterX != finalPosition)
                    {
                        finalPosition = new Vector2(finalPosition.X, potentialYAfterX.Y);
                    }
                }
                else if(attemptedYMove != Position) // If X was blocked, try Y
                {
                    finalPosition = attemptedYMove;
                    // Now try to add X movement from the new Y position
                    Vector2 remainingXMove = new Vector2(deltaMove.X, 0);
                    Vector2 potentialXAfterY = StepUntilBlocked(remainingXMove, map, context, finalPosition);
                    if(potentialXAfterY != finalPosition)
                    {
                        finalPosition = new Vector2(potentialXAfterY.X, finalPosition.Y);
                    }
                }
                // If neither X-only nor Y-only movement was possible,
                // finalPosition remains currentOwnerPosition, meaning no movement.

                Position = finalPosition;

                if (_collider.OnTerrainCollision != null)
                    _collider.OnTerrainCollision(context);
            }
        }
        private Vector2 StepUntilBlocked(Vector2 delta, Map map, IBoardContext context, Vector2? customStartPosition = null)
        {
            const int steps = 10; // You can adjust this for granularity vs. performance
            Vector2 start = customStartPosition ?? Position; // Use custom start or owner's current position
            Vector2 stepIncrement = delta / steps;
            Vector2 currentPosition = start;

            if(_collider == null) return currentPosition + delta;

            for(int i = 1; i <= steps; i++)
            {
                Vector2 testPosition = start + stepIncrement * i;

                // If the next step is clear, update currentPosition and continue
                if(_collider.HasClearPathTo(start, testPosition, context))
                {
                    currentPosition = testPosition;
                }
                else
                {
                    // If the next step is blocked, we've found the limit.
                    // Return the last known clear position (currentPosition).
                    break;
                }
            }
            return currentPosition;
        }

        public void Teleport(Vector2 position, IBoardContext context)
        {
            if(_collider != null && !_collider.CollidesAt(position, context))

                Position = position;
            //throw new NotImplementedException();
        }


        // Ripped from old MobileUnit. NYI
        //internal new void DrawDebug(Graphics g, IDrawContext context)
        //{
        //    base.DrawDebug(g, context);
        //    if(context.DebugSettings.Get(DebugSetting.DrawEntityMovementVector))
        //    {
        //        if(_velocity != Vector2.Zero)
        //        {
        //
        //        }
        //        var pen = context.DrawCache.GetPen(Color.Lime,2);
        //        var endPoint = _position + _velocity*0.1f;
        //        g.DrawLine(pen, _position.ToPointF(), endPoint.ToPointF());
        //    }
        //}
    }
}