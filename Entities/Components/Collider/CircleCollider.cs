using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Util;
namespace Wc3_Combat_Game.Entities.Components.Collider
{

    //internal class CircleCollider: ICollidable
    //{
    //    public WorldPosition _position { get; }
    //    public Func<float> _radius { get; set; }
    //
    //    private readonly bool _sweptCollision;
    //
    //    public Action<IBoardContext>? OnTerrainCollision { get; }
    //
    //    public Vector2 Position => _position.Position;
    //
    //    public float CollisionRadius => _radius();
    //
    //
    //    public CircleCollider(WorldPosition position, Func<float> radius, Action<IBoardContext>? onTerrainCollision = null, bool sweptCollision = false)
    //    {
    //        _radius = radius;
    //        _position = position;
    //        _sweptCollision = sweptCollision;
    //        OnTerrainCollision = onTerrainCollision;
    //    }
    //
    //    public bool HasClearPathTo(Vector2 currentPosition, Vector2 targetPosition, IBoardContext context)
    //    {
    //        if(_sweptCollision)
    //        {
    //            return context.Map.HasLineOfSight(currentPosition, targetPosition, CollisionRadius);
    //        }
    //        else
    //        {
    //            return !CollidesAt(targetPosition, context);
    //        }
    //    }
    //
    //    public void Update(Entity owner, float deltaTime, IBoardContext context)
    //    {
    //        // This method in a collider typically isn't for continuous collision checks
    //        // but for any internal state updates the collider itself might have.
    //        // Collision checks are usually triggered externally (e.g., by a physics system or movement component).
    //        // For now, it can remain empty or be used for future internal logic.
    //        if(OnTerrainCollision != null && context.Map.CollidesAt(_position.Position, CollisionRadius))
    //        {
    //            OnTerrainCollision(context);
    //        }
    //    }
    //
    //    // Checks if this collider, *if placed at 'position'*, would collide with terrain
    //    public bool CollidesAt(Vector2 position, IContext context)
    //    {
    //        return context.Map.CollidesAt(position, _radius());
    //    }
    //
    //    // Overload to allow passing the Entity directly if convenient, extracting its position
    //    public bool CollidesAt(Entity owner, IContext context)
    //    {
    //        return CollidesAt(owner.Position, context);
    //    }
    //
    //    public bool Intersects(ICollidable other)
    //    {
    //        return false;//return GeometryUtils.CollidesCircleWithCircle(this.Position, other.Position,_radius(),other._radius());
    //    }
    //
    //    // Checks collision between this collider (at its owner's position) and another collidable
    //    public void CheckCollision(Entity owner, Entity other, IBoardContext context)
    //    {
    //        // 'as' operator will return null if the cast fails, which is safe for classes.
    //        CircleCollider? otherCircle = other.Collider as CircleCollider;
    //        
    //        if(owner.Mover == null || other.Mover == null) return;
    //        
    //        if(otherCircle != null)
    //        {
    //            // Corrected collision logic from previous discussion
    //            Vector2 seperationVector = GeometryUtils.GetCircleCircleSeperationVector(owner.Position, other.Position, //CollisionRadius,otherCircle.CollisionRadius);
    //            if(seperationVector  != Vector2.Zero)
    //            {
    //                PushApart(owner, other, seperationVector, context);
    //                // This is where you would typically trigger an event or
    //                // call a method on the owner/other to handle the collision response.
    //                // For example:
    //                // owner.HandleCollisionWith(other.Owner); // Assuming other has an Owner property or similar
    //                // Or, if this collider is responsible for pushing:
    //                // PushApart(owner, other.Owner, _radius, otherCircle._radius); // You'd need a method for this
    //            }
    //        }
    //        // You might add checks for other collider types (e.g., BoxCollider) here later
    //    }
    //
    //    // Example of a utility method that might be needed elsewhere or internally
    //    private void PushApart(Entity entityA, Entity entityB, Vector2 separationVector, IBoardContext context)
    //    {
    //        // Logic to calculate collision normal and push entities apart
    //        // This often involves calculating the overlap and moving entities.
    //        IMoveable? moverA = entityA.Mover;
    //        IMoveable? moverB = entityB.Mover;
    //        
    //        Vector2 seperationA = -separationVector;
    //        Vector2 seperationB = separationVector;
    //
    //        if(true)
    //        { }
    //
    //        if(moverA != null && moverB != null)
    //        {
    //            // Both can move, share 50 50.
    //            seperationA *= 0.5f;
    //            seperationB *= 0.5f;
    //        }
    //        else if(moverA != null && moverB == null)
    //        {
    //            // only A can move.
    //            seperationB *= 0f;
    //        }
    //        else if(moverA == null && moverB != null)
    //        {
    //            seperationA *= 0f;
    //        }
    //        else
    //        {
    //            AssertUtil.Assert(false, "Circle Collider Push Apart invalid, Both entities are Immobile.");
    //        }
    //
    //        moverA?.Teleport(entityA.Position + seperationA,context);
    //        moverB?.Teleport(entityB.Position + seperationB,context);
    //        //AssertUtil.Assert(entityA.Collider.Intersects(entityB.Collider));
    //    }
    //}
}