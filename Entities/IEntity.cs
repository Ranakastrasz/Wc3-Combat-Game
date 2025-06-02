using Microsoft.VisualBasic;
using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Base class representing any game object with a position and size.
    /// </summary>
    public abstract class IEntity
    {
        protected float _size; // Diameter
        protected float CollisionRadius => _size / 2; // Hardcode for now.
        protected Vector2 _sizeVector => new Vector2(_size, _size);
        protected Vector2 _position;
        protected Color _fillColor;
        private bool _isAlive = true;
        float _lastKilled = float.NegativeInfinity;
        protected float _despawnDelay = GameConstants.FIXED_DELTA_TIME;

        public TeamType Team;




        public RectangleF BoundingBox { get => _position.RectFFromCenter(_sizeVector); }

        public IEntity(float size, Vector2 position, Color color)
        {
            _size = size;
            _position = position;
            _fillColor = color;
            Team = TeamType.Neutral;
        }


        // Velocity, cooldown, health, mana, etc.
        // Later
        public float Size { get => _size; set => _size = value; }
        public Vector2 Position { get => _position; set => _position = value; }
        public bool IsAlive { get => _isAlive; set => _isAlive = value; }

        public bool IsExpired(IBoardContext context) => !_isAlive && context.CurrentTime > _lastKilled + _despawnDelay;

        public virtual void Draw(Graphics g, IDrawContext context)
        {
            if (!IsAlive) return;
            RectangleF entityRect = _position.RectFFromCenter(_sizeVector);
            using var brush = new SolidBrush(_fillColor);
            g.FillRectangle(brush, entityRect);
        }

        public abstract void Update(float deltaTime, IBoardContext context);

        public void Die(IBoardContext context)
        {
            if (!IsAlive) return;

            _isAlive = false;
            _lastKilled = context.CurrentTime;
        }

        public bool Intersects(IEntity other)
        {
            RectangleF rect = new((PointF)_position, (SizeF)_sizeVector);
            RectangleF otherRect = new((PointF)other._position, (SizeF)other._sizeVector);
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
