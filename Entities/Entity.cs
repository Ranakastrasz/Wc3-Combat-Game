using System.Numerics;

using AStar;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Components.Interface;
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

        public WorldPosition _position = new();

        private IDrawable? _drawer;
        private ICollidable? _collider;

        private IMoveable? _mover;

        public float Radius { get; protected set; } // This will be part of the ICollidable later.
                                                    // And IDraw, seperately, ofc.


        public float Diameter => Radius * 2f;
        protected Vector2 _BoundingSize => new Vector2(Diameter, Diameter);

        public Vector2 Position { get => _position.Position; set => _position.Position = value; }
        private bool _isAlive = true;
        float _lastKilled = float.NegativeInfinity;
        protected float _despawnDelay = GameConstants.FIXED_DELTA_TIME;

        public Team Team;

        // Will include like a dozen interface references,
        // to be built by factory, allowing highly mutable entities.


        public RectangleF BoundingBox { get => Position.RectFFromCenter(_BoundingSize); }

        public bool IsAlive { get => _isAlive; set => _isAlive = value; }
        public ICollidable? Collider { get => _collider; set => _collider = value; }
        public IMoveable? Mover { get => _mover; set => _mover = value; }
        public IDrawable? Drawer { get => _drawer; protected set => _drawer = value; }

        public bool IsExpired(IBoardContext context) => !_isAlive && context.CurrentTime > _lastKilled + _despawnDelay;

        public Entity(float radius, Vector2 position)
        {
            Radius = radius;
            Position = position;
            Drawer = null; //new RectangleDrawable(() => color, () => _position, () => Diameter);

            Team = Team.Neutral;
        }


        public virtual void Draw(Graphics g, IDrawContext context)
        {
            DrawDebug(g, context);

            //if(!IsAlive) return;
            //var brush = context.DrawCache.GetSolidBrush(_fillColor);


            //g.FillRectangle(brush, BoundingBox);
            Drawer?.Draw(g, context);

        }
        internal void DrawDebug(Graphics g, IDrawContext context)
        {
            // Debugging info
            if(context.DebugSettings.Get(DebugSetting.DrawEntityCollisionBox))
            {
                if(Collider != null && Collider.CollidesAt(this, context))
                {
                    var pen = context.DrawCache.GetPen(Color.Red, 1);
                    g.DrawRectangle(pen, BoundingBox.X, BoundingBox.Y, BoundingBox.Width, BoundingBox.Height);
                }
                else
                {
                    var pen = context.DrawCache.GetPen(Color.Yellow, 1);
                    g.DrawRectangle(pen, BoundingBox.X, BoundingBox.Y, BoundingBox.Width, BoundingBox.Height);
                }
            }
        }

        public virtual void Update(float deltaTime, IBoardContext context)
        {

            Mover?.Update(this, deltaTime, context);
            Collider?.Update(this, deltaTime, context);
        }

        public void Die(IBoardContext context)
        {
            if(!IsAlive) return;

            _isAlive = false;
            _lastKilled = context.CurrentTime;
        }

        public bool Intersects(Entity other)
        {
            return BoundingBox.IntersectsWith(other.BoundingBox);
        }

        public float DistanceTo(Entity other)
        {
            Vector2 between = other.Position - Position;
            return between.Length();
        }
        public float DistanceSquaredTo(Entity other)
        {
            Vector2 between = other.Position - Position;
            return between.LengthSquared();
        }
        public float DistanceSquaredTo(Vector2 otherPosition)
        {
            Vector2 between = otherPosition - Position;
            return between.LengthSquared();
        }
        public virtual void OnTerrainCollision(IBoardContext context) // Clearly should be an Event, really.
        {

        }

        public virtual void InitializeInteractionState()
        {
        }


        public virtual void TryInteractWith<T>(T entityB, IBoardContext context) where T : Entity
        {
            if(Collider != null && entityB.Collider != null)
            {
                Collider.CheckCollision(this, entityB, context);
            }

        }
    }
}
