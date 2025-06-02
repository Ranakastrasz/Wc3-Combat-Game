using System.Numerics;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Prototype;
using static Wc3_Combat_Game.Core.GameConstants;
using Wc3_Combat_Game.Core;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Projectile object representing bullets, missiles, or other fired entities.
    /// Inherits from Entity.
    /// </summary>
    public class Projectile : MobileEntity
    {
        //private Vector2 _velocity;
        private float _timeToLive;
        private PrototypeProjectile _prototype;
        public Effect? ImpactEffect => _prototype.ImpactEffect;
        public IEntity? Caster;


        public Projectile(PrototypeProjectile prototype, IEntity? caster, Vector2 position, Vector2 direction): base(prototype.Size, position, prototype.FillColor)
        {
            _prototype = prototype;
            Caster = caster;
            _velocity = GeometryUtils.NormalizeAndScale(direction,prototype.Speed);
            _timeToLive = prototype.Lifespan;

        }

//        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
//        public float TimeToLive { get => _timeToLive; set => _timeToLive = value; }

        public override void Update(float deltaTime, IBoardContext context)
        {
            base.Update(deltaTime, context);
            if (IsAlive)
            {

                _timeToLive -= FIXED_DELTA_TIME;
                if (_timeToLive <= 0)
                {
                    Die(context);
                    _velocity = Vector2.Zero;
                }
            }
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            base.Draw(g, context);
        }

        protected override void OnTerrainCollision(IBoardContext context)
        {
            Die(context);
        }
    }
}
