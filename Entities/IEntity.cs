using Microsoft.VisualBasic;
using System.Numerics;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Base class representing any game object with a position and size.
    /// </summary>
    internal abstract class IEntity
    {
        protected Vector2 _size;
        protected Vector2 _position;
        protected Brush _fillBrush;
        private bool _isAlive = true;
        float _lastKilled = float.NegativeInfinity;
        protected float _despawnDelay = GameConstants.FIXED_DELTA_TIME;

        public RectangleF BoundingBox { get => _position.RectFromCenter(_size); }

        public IEntity(Vector2 size, Vector2 position, Brush brush)
        {
            _size = size;
            _position = position;
            _fillBrush = brush;
        }


        // Velocity, cooldown, health, mana, etc.
        // Later
        public Vector2 Size { get => _size; set => _size = value; }
        public Vector2 Position { get => _position; set => _position = value; }
        public bool IsAlive { get => _isAlive; set => _isAlive = value; }

        public bool IsExpired(double currentTime) => !_isAlive && currentTime > _lastKilled + _despawnDelay;

        public virtual void Draw(Graphics g, float currentTime)
        {
            if (!IsAlive) return;
            Rectangle entityRect = _position.RectFromCenter(_size);
            g.FillEllipse(_fillBrush, entityRect);
            //g.FillRectangle(_fillBrush, _position.RectFromCenter(_size));
        }

        public virtual void Update(float deltaTime, float currentTime)
        { 
            
        }

        public void Die(float currentTime)
        {
            if (!IsAlive) return;

            _isAlive = false;
            _lastKilled = currentTime;
        }

        public bool Intersects(IEntity other)
        {
            RectangleF rect = new((PointF)_position, (SizeF)_size);
            RectangleF otherRect = new((PointF)other._position, (SizeF)other._size);
            return rect.IntersectsWith(otherRect);
        }

        public float DistanceTo(IEntity other)
        {
            Vector2 between = other.Position - _position;
            return between.Length();
        }
        public float DistanceSquaredTo(IEntity other)
        {
            Vector2 between = other.Position - _position;
            return between.LengthSquared();
        }
    }
}
