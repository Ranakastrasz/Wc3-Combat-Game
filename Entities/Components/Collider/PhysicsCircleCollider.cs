using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using AStar;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;

namespace Wc3_Combat_Game.Entities.Components.Collider
{
    //class PhysicsCircleCollider: ICollidable
    //{
    //    private Body _body;
    //    public Action<IBoardContext>? OnTerrainCollision { get; }
    //
    //    public PhysicsCircleCollider(World world, Vector2 position, float radius, Action<IBoardContext>? onTerrainCollision = null)
    //    {
    //        _body = world.CreateBody(position, 0, BodyType.Dynamic);
    //        var fixture = _body.CreateCircle(radius, density: 1f);
    //        fixture.Restitution = 0.2f;
    //        fixture.Friction = 0.3f;
    //        OnTerrainCollision = onTerrainCollision;
    //    }
    //
    //    public Vector2 Position
    //    {
    //        get => _body.Position.ToNumerics();
    //        set => _body.Position = value;
    //    }
    //
    //    public Vector2 Velocity
    //    {
    //        get => _body.LinearVelocity.ToNumerics();
    //        set => _body.LinearVelocity = value;
    //    }
    //
    //    public void ApplyForce(Vector2 force)
    //    {
    //        _body.ApplyForce(force);
    //    }
    //
    //    public void Dispose()
    //    {
    //        _body.World.Remove(_body);
    //    }
    //
    //    public float CollisionRadius => _body.FixtureList[0].Shape.Radius;
    //    public void Update(Entity owner, float deltaTime, IBoardContext context)
    //    {
    //        if(OnTerrainCollision != null && context.Map.CollidesAt(Position, CollisionRadius))
    //        {
    //            OnTerrainCollision(context);
    //        }
    //    }
    //
    //    public bool CollidesAt(Entity owner, IContext context) => false;
    //
    //    public bool CollidesAt(Vector2 position, IContext context) => false;
    //
    //    public void CheckCollision(Entity owner, Entity other, IBoardContext context) { }
    //
    //    public bool HasClearPathTo(Vector2 start, Vector2 end, IBoardContext context) =>
    //        throw new NotSupportedException("Physics-based collider does not support HasClearPathTo directly.");
    //}
}