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
    //public class UnitMover: IMoveable
    //{
    //    public ICollidable? Collider { get; private set; }
    //    private Vector2 _position;
    //    private Vector2 _velocity;
    //    public Vector2 Position => Collider != null ? Collider.Position : _position;
    //
    //    public Vector2 Velocity => Collider != null ? Collider.Velocity : _velocity;
    //
    //    Vector2 IMoveable.Velocity { get => Velocity; set => throw new NotImplementedException(); }
    //
    //    public void Teleport(Vector2 position, IBoardContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //
    //    public void Update(Entity owner, float deltaTime, IBoardContext context)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}


    //public class UnitMover: IMoveable
    //{
    //    ICollidable? _collider;
    //    WorldPosition _position;
    //    Vector2 Position { get => _position.Position; set => _position.Position = value; }
    //    public Vector2 Velocity { get; set; }
    //
    //    // Tempory, psudobuff crap.
    //    public float SlowExpires { get; set; } = float.NegativeInfinity;
    //
    //    public UnitMover(WorldPosition position, ICollidable collider, Vector2? velocity = null)
    //    {
    //        _position = position;
    //        _collider = collider;
    //        Velocity = velocity ?? Vector2.Zero;
    //    }
    //
    //    public void Update(Entity owner, float deltaTime, IBoardContext context)
    //    {
    //        Vector2 deltaMove = Velocity * deltaTime;
    //
    //        if(!TimeUtils.HasElapsed(context.CurrentTime, SlowExpires, 0f))
    //        {
    //            deltaMove *= 0.75f;
    //        }
    //
    //        if(_collider == null)
    //        {
    //            Position += deltaMove;
    //            return;
    //        }
    //        else
    //        {
    //            Map map = context.Map;
    //            AssertUtil.NotNull(map);
    //
    //            Vector2 currentPosition = Position;
    //            Vector2 targetPosition = currentPosition + deltaMove;
    //
    //            // Attempt full movement first
    //            if(_collider.HasClearPathTo(owner, targetPosition, context))
    //            {
    //                Position = targetPosition;
    //                return;
    //            }
    //
    //            // If full movement is blocked, attempt to slide.
    //            // This is where the main change will be.
    //            // Instead of simple X-only or Y-only, we'll try a more robust sliding approach.
    //
    //            Vector2 remainingMovement = deltaMove;
    //            int maxIterations = 3; // Prevent infinite loops, adjust as needed
    //
    //            for(int i = 0; i < maxIterations; i++)
    //            {
    //                Vector2 newPotentialPosition = currentPosition + remainingMovement;
    //
    //                // Check if the current remaining movement can be fully applied
    //                if(_collider.HasClearPathTo(owner, newPotentialPosition, context))
    //                {
    //                    currentPosition = newPotentialPosition;
    //                    remainingMovement = Vector2.Zero; // All movement applied
    //                    break;
    //                }
    //                else
    //                {
    //                    // If blocked, find the point just before collision and
    //                    // calculate a "slide" vector.
    //
    //                    // This part is crucial and requires a more detailed collision test.
    //                    // HasClearPathTo only tells you "yes" or "no".
    //                    // You need a function that returns the *point of collision* or the *normal* of the hit surface.
    //
    //                    // For a circular collider, if it hits a wall, the slide direction
    //                    // is perpendicular to the collision normal.
    //
    //                    // Placeholder for advanced collision logic:
    //                    // Let's assume you have a method like TryMoveAndSlide that
    //                    // returns the actual movement done and the remaining slide vector.
    //                    Vector2 actualMoveThisIteration = Vector2.Zero;
    //                    Vector2 slideVector = Vector2.Zero;
    //
    //                    // This is a simplified concept. In reality, you'd iterate
    //                    // and resolve against each collision detected.
    //                    // For circular colliders and rectangular terrain, you'd check
    //                    // for axis-aligned collisions and corner collisions.
    //
    //                    // Option 1: Try X then Y, or Y then X as a fallback, but make it smarter
    //                    // The current StepUntilBlocked is good for finding the limit on one axis.
    //
    //                    Vector2 tempPositionX = StepUntilBlocked(new Vector2(remainingMovement.X, 0), map, context, currentPosition);
    //                    Vector2 tempPositionY = StepUntilBlocked(new Vector2(0, remainingMovement.Y), map, context, currentPosition);
    //
    //                    // If we can move further on X
    //                    if(Vector2.DistanceSquared(currentPosition, tempPositionX) > Vector2.DistanceSquared(currentPosition, tempPositionY))
    //                    {
    //                        actualMoveThisIteration = tempPositionX - currentPosition;
    //                        currentPosition = tempPositionX;
    //                        remainingMovement.Y = 0; // Clear Y component if X was prioritized
    //                    }
    //                    else // Prioritize Y or if X was no better
    //                    {
    //                        actualMoveThisIteration = tempPositionY - currentPosition;
    //                        currentPosition = tempPositionY;
    //                        remainingMovement.X = 0; // Clear X component if Y was prioritized
    //                    }
    //
    //                    if(actualMoveThisIteration.LengthSquared() < 0.0001f && remainingMovement.LengthSquared() > 0.0001f)
    //                    {
    //                        // If no movement was made this iteration but there's still remaining movement,
    //                        // it means we are truly stuck in this direction. Break to prevent infinite loop.
    //                        break;
    //                    }
    //                }
    //            }
    //
    //            Position = currentPosition;
    //
    //            if(_collider.OnTerrainCollision != null)
    //                _collider.OnTerrainCollision(context);
    //        }
    //    }
    //    private Vector2 StepUntilBlocked(Vector2 delta, Map map, IBoardContext context, Vector2? customStartPosition = null)
    //    {
    //        const int steps = 10; // You can adjust this for granularity vs. performance
    //        Vector2 start = customStartPosition ?? Position; // Use custom start or owner's current position
    //        Vector2 stepIncrement = delta / steps;
    //        Vector2 currentPosition = start;
    //
    //        if(_collider == null) return currentPosition + delta;
    //
    //        for(int i = 1; i <= steps; i++)
    //        {
    //            Vector2 testPosition = start + stepIncrement * i;
    //
    //            // If the next step is clear, update currentPosition and continue
    //            if(_collider.HasClearPathTo(start, testPosition, context))
    //            {
    //                currentPosition = testPosition;
    //            }
    //            else
    //            {
    //                // If the next step is blocked, we've found the limit.
    //                // Return the last known clear position (currentPosition).
    //                break;
    //            }
    //        }
    //        return currentPosition;
    //    }
    //
    //    public void Teleport(Vector2 position, IBoardContext context)
    //    {
    //        if(_collider != null && !_collider.CollidesAt(position, context))
    //
    //            Position = position;
    //        //throw new NotImplementedException();
    //    }
    //
    //
    //    // Ripped from old MobileUnit. NYI
    //    //internal new void DrawDebug(Graphics g, IDrawContext context)
    //    //{
    //    //    base.DrawDebug(g, context);
    //    //    if(context.DebugSettings.Get(DebugSetting.DrawEntityMovementVector))
    //    //    {
    //    //        if(_velocity != Vector2.Zero)
    //    //        {
    //    //
    //    //        }
    //    //        var pen = context.DrawCache.GetPen(Color.Lime,2);
    //    //        var endPoint = _position + _velocity*0.1f;
    //    //        g.DrawLine(pen, _position.ToPointF(), endPoint.ToPointF());
    //    //    }
    //    //}
    //}
}