using System.Numerics;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities.Components
{

    public class PhysicsObject
    {
        private Body _body;

        public PhysicsObject(Object owner, World world, Vector2 position, float radius, bool isDynamic)
        {
            _body = world.CreateBody(position/64f, 0,
                isDynamic ? BodyType.Dynamic : BodyType.Static);

            _body.CreateCircle(radius, 1f);
            _body.Tag = owner;
        }

        public Body Body { get => _body; set => _body = value; }
        public Vector2 Position
        {
            get => _body.Position.ToNumerics()*64f;
            set => _body.Position = value/64f; // rarely used, maybe for teleport
        }

        public Vector2 Velocity
        {
            get => _body.LinearVelocity.ToNumerics()*64f;
            set => _body.LinearVelocity = value/64f;
        }

        public void ApplyForce(Vector2 force) =>
            _body.ApplyForce(force);

        // ICollidable
        public float CollisionRadius => _body.FixtureList[0].Shape.Radius;

        public bool CollidesAt(Vector2 pos, IContext context) => false; // or implement via test body

        public void Update(Entity owner, float dt, IBoardContext context) { }

        public Action<IBoardContext>? OnTerrainCollision => null;

        public bool HasClearPathTo(Vector2 a, Vector2 b, IBoardContext ctx) => false;

        public void CheckCollision(Entity owner, Entity other, IBoardContext context) { }

    }
}