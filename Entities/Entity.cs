using Microsoft.VisualBasic;
using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Base class representing any game object with a position and size.
    /// </summary>
    public class Entity
    {
        public readonly int Index = s_NextIndex++;
        private static int s_NextIndex = 0;

        public float Radius { get; protected set; } // This will be part of the ICollidable later.
                                                    // And IDraw, seperately, ofc.
        public float Diameter => Radius * 2f;
        protected Vector2 _BoundingSize => new Vector2(Diameter, Diameter);

        protected Vector2 _position;
        protected Color _fillColor;
        private bool _isAlive = true;
        float _lastKilled = float.NegativeInfinity;
        protected float _despawnDelay = GameConstants.FIXED_DELTA_TIME;

        public TeamType Team;

        // Will include like a dozen interface references,
        // to be built by factory, allowing highly mutable entities.


        public RectangleF BoundingBox { get => _position.RectFFromCenter(_BoundingSize); }

        public Entity(float radius, Vector2 position, Color color)
        {
            Radius = radius;
            _position = position;
            _fillColor = color;
            Team = TeamType.Neutral;
        }


        // Velocity, cooldown, health, mana, etc.
        // Later
        public Vector2 Position { get => _position; set => _position = value; }
        public bool IsAlive { get => _isAlive; set => _isAlive = value; }

        public bool IsExpired(IBoardContext context) => !_isAlive && context.CurrentTime > _lastKilled + _despawnDelay;

        public virtual void Draw(Graphics g, IDrawContext context)
        {
            DrawDebug(g, context);

            if (!IsAlive) return;
            var brush = context.DrawCache.GetOrCreateBrush(_fillColor);


            g.FillRectangle(brush, BoundingBox);


        }
        internal void DrawDebug(Graphics g, IDrawContext context)
        {
            // Debugging info
            if(context.DebugSettings.Get(DebugSetting.DrawEntityCollisionBox))
            {
                using var pen = new Pen(Color.Yellow, 1);
                g.DrawRectangle(pen, BoundingBox.X, BoundingBox.Y, BoundingBox.Width, BoundingBox.Height);
            }
        }

        public virtual void Update(float deltaTime, IBoardContext context)
        {
            // Call all interfaces here. For now, nothing to do.
        }

        public void Die(IBoardContext context)
        {
            if (!IsAlive) return;

            _isAlive = false;
            _lastKilled = context.CurrentTime;
        }

        public bool Intersects(Entity other)
        {
            return BoundingBox.IntersectsWith(other.BoundingBox);
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
        public float DistanceSquaredTo(Vector2 otherPosition)
        {
            Vector2 between = otherPosition - _position;
            return between.LengthSquared();
        }

    }
}
