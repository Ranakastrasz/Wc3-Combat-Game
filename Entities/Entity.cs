using System.Numerics;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities
{
    public class Entity
    {
        protected Vector2 _size;
        protected Vector2 _position;
        protected Brush _brush;
        protected bool _isAlive = false;

        public Entity(Vector2 size, Vector2 position, Brush brush)
        {
            _size = size;
            _position = position;
            _brush = brush;
        }


        // Velocity, cooldown, health, mana, etc.
        // Later
        public Vector2 Size { get => _size; set => _size = value; }
        public Vector2 Position { get => _position; set => _position = value; }
        
        public void Draw(Graphics g)
        {
            g.FillRectangle(_brush, _position.RectFromCenter(_size));
        }

        public virtual void Update()
        { 
            
        }

        public bool Intersects(Entity other)
        {
            Rect rect = new (_position, _size);
            Rect otherRect = new(other._position, other._size);
            return rect.Intersects(otherRect);
        }

        public float DistanceTo(Entity other)
        {
            Vector2 between = other.Position - _position;
            return between.Length();
        }
        public float DistanceSquaredTo(Entity other)
        {
            Vector2 between = other.Position - _position;
            return between.LengthSquared();
        }
    }
}
