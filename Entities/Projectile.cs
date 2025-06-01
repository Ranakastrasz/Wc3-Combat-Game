using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Prototypes;
using static Wc3_Combat_Game.GameConstants;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Projectile object representing bullets, missiles, or other fired entities.
    /// Inherits from Entity.
    /// </summary>
    internal class Projectile : IEntity
    {
        private Vector2 _velocity;
        private float _timeToLive;
        //private ProjectilePrototype _prototype;
        public Effect ImpactEffect;
        public IEntity Caster;

        public Projectile(IEntity caster, Vector2 position, Vector2 size, Brush brush, Vector2 velocity, float timeToLive, Effect impactEffect) : base(size,position,brush)
        {
            Caster = caster;
            _velocity = velocity;
            _timeToLive = timeToLive;
            ImpactEffect = impactEffect;
        }

        //public Projectile(Vector2 position, Vector2 direction, ProjectilePrototype prototype) : base(position, prototype.size, prototype.brush)
        //{
        //    //_velocity = prototype 
        //    _position = position;
        //    _prototype = prototype;
        //}

        public Vector2 Velocity { get => _velocity; set => _velocity = value; }
        public float TimeToLive { get => _timeToLive; set => _timeToLive = value; }

        public override void Update(float deltaTime, float currentTime)
        {
            if (IsAlive)
            {
                _position += _velocity * FIXED_DELTA_TIME;

                _timeToLive -= FIXED_DELTA_TIME;

                if (_timeToLive <= 0)
                    Die(currentTime);
            }
        }

        public override void Draw(Graphics g, float currentTime)
        {
            base.Draw(g, currentTime);
        }
    }
}
