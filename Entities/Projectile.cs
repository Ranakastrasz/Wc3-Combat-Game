using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Util;
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

        public Projectile(Vector2 size, Vector2 position, Brush brush, Vector2 velocity, float timeToLive) : base(size,position,brush)
        {
            _velocity = velocity;
            _timeToLive = timeToLive;
        }

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
