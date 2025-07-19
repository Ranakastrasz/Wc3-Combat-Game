using System.Numerics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components;
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

        public new ProjectileMover Mover { get; }
        public new ICollidable Collider { get; }


        public Projectile(ProjectilePrototype prototype, Entity? caster, Vector2 position, Vector2 direction) : base(prototype.Radius, position)
        {
            _prototype = prototype;
            Caster = caster;
            _timeToLive = prototype.Lifespan;

            Mover = new ProjectileMover(_position, GeometryUtils.NormalizeAndScale(direction, prototype.Speed))
            {
                Velocity = GeometryUtils.NormalizeAndScale(direction, prototype.Speed)
            };
            base.Mover = Mover;

            Drawer = new PolygonDrawable((context) => _prototype.FillColor, () => Position, () => _prototype.Radius*2, () => 1, () => IsAlive);

            Collider = new CircleCollider(_position, () => _prototype.Radius, true);
            base.Collider = Collider;
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
                    Mover.Velocity = Vector2.Zero;
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
