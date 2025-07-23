using System.Numerics;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components;
using Wc3_Combat_Game.Entities.Components.Collider;
using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Movement;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Util;

using static Wc3_Combat_Game.Core.GameConstants;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Projectile object representing bullets, missiles, or other fired entities.
    /// Inherits from Entity.
    /// </summary>
    public class Projectile: Entity
    {
        //private Vector2 _velocity;
        private float _timeToLive;
        private ProjectilePrototype _prototype;
        public IGameplayAction? ImpactEffect => _prototype.ImpactEffect;
        public Entity? Caster;

        //public new ProjectileMover Mover { get; }
        //public new ICollidable Collider { get; }


        public Projectile(ProjectilePrototype prototype, Entity? caster, Vector2 position, Vector2 direction, IBoardContext context) : base(prototype.Radius, position, context)
        {
            _prototype = prototype;
            Caster = caster;
            _timeToLive = prototype.Lifespan;
            Drawer = new PolygonDrawable((context) => _prototype.FillColor, () => Position, () => _prototype.Radius * 2, () => 1, () => IsAlive);

            //Mover = new ProjectileMover(_position, GeometryUtils.NormalizeAndScale(direction, prototype.Speed))
            //{
            //    Velocity = GeometryUtils.NormalizeAndScale(direction, prototype.Speed)
            //};
            //base.Mover = Mover;
            //Collider = new CircleCollider(_position, () => _prototype.Radius,(context) => OnTerrainCollision(context) , true);
            //Collider = new PhysicsCircleCollider(context.PhysicsWorld, position, _prototype.Radius, (context) => OnTerrainCollision(context));
            //base.Collider = Collider;

            _physicsObject.Velocity = GeometryUtils.NormalizeAndScale(direction, prototype.Speed);
            Body body = _physicsObject.Body;
            if(body.FixtureList.Count > 0)
            {
                Fixture fixture = body.FixtureList[0];
                fixture.CollisionCategories = PhysicsManager.PlayerProjectileCategory;
                fixture.CollidesWith = PhysicsManager.EnemyCategory;
                fixture.IsSensor = true;
                body.LinearDamping = 0f;
                fixture.Friction = 0f;
                fixture.Restitution = 0f; // unless bounce is needed
            }
            Console.WriteLine($"Velocity: {body.LinearVelocity}, Position: {body.Position}");
            Console.WriteLine($"Mass: {body.Mass}");
            Console.WriteLine($"Inertia: {body.Inertia}");

            //fixture.OnCollision += (f1, f2, contact) =>
            //{
            //    var myEntity = (Entity)f1.Body.Tag;
            //    var otherEntity = (Entity)f2.Body.Tag;
            //
            //    if(ShouldIgnore(myEntity, otherEntity))
            //        return false; // skip collision
            //
            //    return true; // allow it
            //};

        }

        //        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
        //        public float TimeToLive { get => _timeToLive; set => _timeToLive = value; }

        public override void Update(float deltaTime, IBoardContext context)
        {
            base.Update(deltaTime, context);
            if(IsAlive)
            {

                _timeToLive -= FIXED_DELTA_TIME;
                if(_timeToLive <= 0)
                {
                    Die(context);
                    //Mover.Velocity = Vector2.Zero;
                }
            }
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            DrawDebug(g, context);
            base.Draw(g, context);

        }

        public override void OnTerrainCollision(IBoardContext context)
        {
            Die(context);
        }
    }
}
