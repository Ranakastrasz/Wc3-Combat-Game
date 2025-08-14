using System.Numerics;

using AStar;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Collider;
using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Nebula;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Base class representing any game object with a position and size.
    /// </summary>
    public class Entity: IDisposable
    {
        public readonly int Index = s_NextIndex++;
        private static int s_NextIndex = 0;


        protected IDrawable? _drawer;

        protected PhysicsBodyComponent _physicsBody;

        public float Radius => _physicsBody.CollisionRadius;

        public float Diameter => Radius * 2f;
        protected Vector2 _BoundingSize => new Vector2(Diameter, Diameter);

        public Vector2 Position { get => _physicsBody.Position; set => _physicsBody.Position = value; }
        private bool _isAlive = true;
        float _lastKilled = float.NegativeInfinity;
        protected float _despawnDelay = GameConstants.FIXED_DELTA_TIME;

        public Team Team;

        // Will include like a dozen interface references,
        // to be built by factory, allowing highly mutable entities.


        public RectangleF BoundingBox { get => Position.RectFFromCenter(_BoundingSize); }

        public bool IsAlive { get => _isAlive; set => _isAlive = value; }
        //public ICollidable? Collider { get => _collider; set => _collider = value; }
        //public IMoveable? Mover { get => _mover; set => _mover = value; }
        public IDrawable? Drawer { get => _drawer; protected set => _drawer = value; }
        public PhysicsBodyComponent PhysicsBody { get => _physicsBody; protected set => _physicsBody = value; }

        public bool IsExpired(IBoardContext context) => !_isAlive && context.CurrentTime > _lastKilled + _despawnDelay;

        public Entity(float radius, Vector2 position, IBoardContext context)
        {
            Drawer = null;

            Team = Team.Neutral;
            _physicsBody = new PhysicsBodyComponent(this, context.PhysicsWorld, position, radius);
        }


        public virtual void Draw(Graphics g, IDrawContext context)
        {

            //if(!IsAlive) return;
            //var brush = context.DrawCache.GetSolidBrush(_fillColor);


            //g.FillRectangle(brush, BoundingBox);
            Drawer?.Draw(g, context);

            DrawDebug(g, context);

        }
        internal virtual void DrawDebug(Graphics g, IDrawContext context)
        {
            Drawer?.DrawDebug(g, context);
            _physicsBody.DrawDebug(g, context);
        }

        public virtual void Update(float deltaTime, IBoardContext context)
        {
            _physicsBody.Update(deltaTime, context);
            //Mover?.Update(this, deltaTime, context);
            //Collider?.Update(this, deltaTime, context);
        }

        public virtual void Die(IBoardContext context)
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
        public virtual void OnTerrainCollision(Tile tile, Vector2 impactPoint, IBoardContext context)
        {

        }

        public virtual void InitializeInteractionState()
        {
        }


        public virtual void TryInteractWith<T>(T entityB, IBoardContext context) where T : Entity
        {
            //if(Collider != null && entityB.Collider != null)
            //{
            //    Collider.CheckCollision(this, entityB, context);
            //}

        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            if(_physicsBody != null)
            {
                _physicsBody.Dispose();
                _physicsBody = null!;
            }
            if(_drawer != null)
            {
                _drawer.Dispose();
                _drawer = null!;
            }
        }
    }
}
