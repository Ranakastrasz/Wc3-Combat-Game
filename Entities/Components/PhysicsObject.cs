using System.Numerics;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities.Components
{

    public class PhysicsObject
    {
        private Body _body;

        private static readonly float SCALAR = 64f;
        public PhysicsObject(Object owner, World world, Vector2 position, float radius, bool isDynamic)
        {
            _body = world.CreateBody(position/ SCALAR, 0,
                isDynamic ? BodyType.Dynamic : BodyType.Static);

            _body.CreateCircle(radius/SCALAR, 1f);
            _body.Tag = owner;
        }

        public Body Body { get => _body; set => _body = value; }
        public Vector2 Position
        {
            get => _body.Position.ToNumerics()* SCALAR;
            set => _body.Position = value/ SCALAR;
        }

        public Vector2 Velocity
        {
            get => _body.LinearVelocity.ToNumerics()* SCALAR;
            set => _body.LinearVelocity = value/ SCALAR;
        }

        public void ApplyForce(Vector2 force) =>
            _body.ApplyForce(force);

        // ICollidable
        public float CollisionRadius => _body.FixtureList[0].Shape.Radius * SCALAR;

        public bool CollidesAt(Vector2 pos, IContext context) => false; // temp, to remove

        public void Update(Entity owner, float dt, IBoardContext context) { }

        public Action<IBoardContext>? OnTerrainCollision => null;

        public bool HasClearPathTo(Vector2 a, Vector2 b, IBoardContext ctx) => false;

        public void CheckCollision(Entity owner, Entity other, IBoardContext context) { }

    }
}