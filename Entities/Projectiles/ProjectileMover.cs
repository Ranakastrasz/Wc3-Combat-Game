using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;

namespace Wc3_Combat_Game.Entities.Projectiles
{
    public class ProjectileMover: IMoveable
    {
        private WorldPosition _position;
        private Vector2 _velocity;
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public WorldPosition Position { get => _position; }

        public ProjectileMover(WorldPosition position, Vector2 velocity)
        { 
            Velocity = velocity;
            _position = position;
            
        }

        public void Teleport(Vector2 position, IBoardContext context)
        {
            _position.Position = position;
        }

        public void Update(Entity owner, float deltaTime, IBoardContext context)
        {
            _position.Position += _velocity * deltaTime;
        }
    }
}