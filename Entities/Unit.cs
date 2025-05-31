using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Util;


namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Represents a living or interactive game unit with health and actions.
    /// Inherits from Entity.
    /// </summary>
    internal class Unit : IEntity
    {
        public float Health {  get; protected set; }

        float _lastDamaged = float.NegativeInfinity;
        static float s_DamageFlashTime = GameConstants.FIXED_DELTA_TIME;
        protected Vector2 _moveVector = Vector2.Zero;

        public Unit(Vector2 size, Vector2 position, Brush brush) : base(size, position, brush)
        {
            _despawnDelay = 1f; 
        }

        public override void Draw(Graphics g, float currentTime)
        {
            Rectangle entityRect = _position.RectFromCenter(_size);



            if (currentTime < _lastDamaged + s_DamageFlashTime)
            {
                g.FillEllipse(Brushes.White, entityRect);
            }
            else if (IsAlive)
            {
                g.FillEllipse(_fillBrush, entityRect);
            }
            else
            {
                g.FillEllipse(Brushes.Gray, entityRect);
            }
        }

        public void Damage(float amount, float currentTime)
        {
            Health -= amount;
            _lastDamaged = currentTime;
            if (Health <= 0f)
            {
                Health = 0;
                Die(currentTime);
            }
        }
    }
}
