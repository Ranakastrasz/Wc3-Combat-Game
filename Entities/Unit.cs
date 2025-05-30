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
    public abstract class Unit : Entity
    {
        public float _health {  get; protected set; }

        float _lastDamaged = float.NegativeInfinity;
        static float _DamageFlashTime = GameConstants.FIXED_DELTA_TIME;

        public Unit(Vector2 size, Vector2 position, Brush brush) : base(size, position, brush)
        {
            _removalDelay = 1f; 
        }

        public override void Draw(Graphics g)
        {
            float GlobalTime = GameManager.Instance.GlobalTime;
            Rectangle entityRect = _position.RectFromCenter(_size);



            if (GlobalTime < _lastDamaged + _DamageFlashTime)
            {
                g.FillRectangle(Brushes.White, entityRect);
            }
            else if (IsAlive)
            {
                g.FillRectangle(_brush, entityRect);
            }
            else
            {
                g.FillRectangle(Brushes.Gray, entityRect);
            }
        }

        public void Damage(float amount)
        {
            _health -= amount;
            _lastDamaged = GameManager.Instance.GlobalTime;
            if (_health <= 0f)
            {
                _health = 0;
                Die();
            }
        }
    }
}
