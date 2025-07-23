using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities.Components.Collider
{
    public class StaticSquareCollider
    {
        private Body _body;

        public StaticSquareCollider(Object owner, World world, Vector2 position, float width, float height, bool isDynamic)
        {
            _body = world.CreateBody(position / 64f, 0,
                isDynamic ? BodyType.Dynamic : BodyType.Static);

            _body.CreateRectangle(width, height, 1f, new Vector2(width/2,height/2));
            _body.Tag = owner;
        }

        public Body Body { get => _body; set => _body = value; }
        public Vector2 Position
        {
            get => _body.Position.ToNumerics() * 64f;
            set => _body.Position = value / 64f; // rarely used, maybe for teleport
        }


        // ICollidable
        public float CollisionRadius => _body.FixtureList[0].Shape.Radius;
        public void Update(Entity owner, float dt, IBoardContext context) { }

        public Action<IBoardContext>? OnTerrainCollision => null;
        public void CheckCollision(Entity owner, Entity other, IBoardContext context) { }

    }
}