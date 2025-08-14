using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;

using Color = System.Drawing.Color;



namespace Wc3_Combat_Game.Entities.Components.Nebula
{
    /// <summary>
    /// Wrapper for the Aether Body.
    /// Also handles (for now) the collision callbacks, and velocity, using pure kinematic motion,
    /// Since you cannot get the desired behavior using Dynamic bodies.
    /// </summary>
    public class PhysicsBodyComponent
    {
        private Body _body;
        private Vector2 _velocity; // Scaled by GameConstants.PHYSICS_SCALE
        //private ICollider? _collider;
        private Fixture? _collider => _body.FixtureList.Count > 0 ? _body.FixtureList[0] : null;


        public System.Numerics.Vector2 Velocity
        {
            get => _velocity.ToNumerics() / GameConstants.PHYSICS_SCALE;
            set => _velocity = value * GameConstants.PHYSICS_SCALE;
        }
        public System.Numerics.Vector2 Position
        {
            get => _body.Position.ToNumerics() / GameConstants.PHYSICS_SCALE;
            set => _body.Position = value * GameConstants.PHYSICS_SCALE;
        }


        private float _collisionRadius
        {
            get => _collider?.Shape.Radius ?? 0f;
            set
            {
                if(_collider != null)
                {
                    _body.Remove(_collider);
                }
                if(value > 0f)
                {
                    _body.CreateCircle(value, density: 1f, offset: Vector2.Zero);
                }
                else
                {
                    // Not sure if this is correct. Might want set 0 to disable collision instead.
                    _body.CreateCircle(0.01f, density: 1f, offset: Vector2.Zero);
                }
            }
        }
        public float CollisionRadius
        {
            get => _collisionRadius / GameConstants.PHYSICS_SCALE;
            set
            {
                 if (value < 0f)
                      throw new ArgumentOutOfRangeException(nameof(value), "Collision radius cannot be negative.");
                 _collisionRadius = value * GameConstants.PHYSICS_SCALE;
            }
        }

        public void Accelerate(System.Numerics.Vector2 acceleration, float deltaTime)
        {
            // Update velocity based on acceleration.
            Velocity += acceleration * deltaTime;
        }
        public void Impulse(System.Numerics.Vector2 impulse)
        {
            Velocity += impulse;
        }
        public void Teleport(System.Numerics.Vector2 position, IBoardContext context)
        {
            Position = position;
            // Will add validation when I figure out how, and I have a reason to care, i.e. actually need to use this function, and not just as a SetLocation
            //if(_body.FixtureList[0].!= null && !Collider.CollidesAt(position, context))
            //{
            //    Position = position;
            //}
            //else
            //{
            //    throw new InvalidOperationException("Cannot teleport to a position that collides with terrain.");
            //}
        }

        public PhysicsBodyComponent(object owner, World world, System.Numerics.Vector2 position, float radius)
        {

            _body = world.CreateBody(position * GameConstants.PHYSICS_SCALE, 0, BodyType.Dynamic);
            _body.Tag = owner;
            CollisionRadius = radius;
            if (CollisionRadius > 0f)
            {
                // Create a circular fixture for the body.
                _body.CreateCircle(_collisionRadius, density: 1f, offset: Vector2.Zero);
            }
            else
            {
                // Create a fixture with no radius.
                _body.CreateCircle(0.01f, density: 1f, offset: Vector2.Zero);
            }

            //_collider.CollidesWith

        }
        
        /// <summary>
        /// Body does not respect normal game scale.
        /// </summary>
        public Body Body => _body;

        public void Update(float deltaTime, IContext context)
        {
            // Update body velocity.
            _body.LinearVelocity = _velocity;

            // Include other velocity crap later.
            
        }

        public void Dispose()
        {
            if(_body.World != null)
                _body.World.Remove(_body);
            _body = null!;
        }


        internal void DrawDebug(Graphics g, IDrawContext context)
        {
            if(_body == null || _body.World == null)
                return;
            if(context.DebugSettings[IO.DebugSetting.DrawEntityCollisionBox])
            {
                // Draw the circle collider
                var position = Position;
                var diameter = CollisionRadius*2;
                g.DrawEllipse(context.DrawCache.GetPen(Color.Red), position.X - diameter / 2, position.Y - diameter / 2, diameter, diameter);
                // Draw the body position
                //g.DrawEllipse(context.DrawCache.GetPen(Color.Red),position, 2f);
            }
        }
    }
}