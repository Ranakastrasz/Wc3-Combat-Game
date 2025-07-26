
using System.Numerics;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities.Components.Collider
{
    public class StaticSquareCollider
    {
        private Body _body;

        private static readonly float SCALAR = 64f;

        public SizeF CollisionSize { get; private set; }

        public StaticSquareCollider(Object owner, World world, Vector2 position, float width, float height)
        {
            CollisionSize = new SizeF(width, height);
            width /= SCALAR;
            height /= SCALAR;
            position /= SCALAR;
            _body = world.CreateBody(position, 0,
                BodyType.Static);

            _body.CreateRectangle(width, height, 1f, new Vector2(width/2,height/2));
            _body.Tag = owner;
            if(_body.FixtureList.Count > 0)
            {
                Fixture fixture = _body.FixtureList[0];
                fixture.CollisionCategories = PhysicsManager.TerrainCategory;
                fixture.CollidesWith = PhysicsManager.PlayerCategory;
            }

        }

        public Body Body { get => _body; set => _body = value; }
        public Vector2 Position
        {
            get => _body.Position.ToNumerics() * SCALAR;
            set => _body.Position = value / SCALAR;
        }


        // ICollidable
        public float CollisionRadius => _body.FixtureList[0].Shape.Radius;
        public void Update(Entity owner, float dt, IBoardContext context) { }

        public Action<IBoardContext>? OnTerrainCollision => null;
        public void CheckCollision(Entity owner, Entity other, IBoardContext context) { }

    }
}