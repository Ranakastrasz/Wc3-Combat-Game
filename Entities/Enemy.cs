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
    /// AI-controlled enemy unit with behavior logic.
    /// Inherits from Unit.
    /// </summary>
    internal class Enemy : Unit
    {
        private Vector2 _velocity = Vector2.Zero;
        private Player _target; // Technically only 1 player atm, but will change.
        private float _speed;


        public Enemy(Vector2 size, Vector2 position, Brush brush, Player target, float speed) : base(size, position, brush)
        {
            _target = target;
            _speed = speed;

            _health = 100f;
        }
        public Vector2 Velocity { get => _velocity; set => _velocity = value; }

        public override void Update()
        {
            if (!IsAlive) return;
            // Target player. Calculate distnace/angle crap, then set velocity to close distance.
            // _velocity = normalizeAndScale(Line between _position and player._position

            _velocity = GeometryUtils.NormalizeAndScale(_target.Position - Position, _speed);

            _position += _velocity * FIXED_DELTA_TIME;

            // Collision not calculated here. Collision between multiple enemies (crowding), projectiles, and enemy. And also bounds. 
        }
    }
}
