using Accessibility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Prototypes;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;


namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Represents a living or interactive game unit with health and actions.
    /// Inherits from Entity.
    /// </summary>
    internal class Unit : IEntity
    {

        public IUnitController? Controller = null;
        public Unit? Target { get; set; }

        public IWeapon Weapon;

        public float MaxHealth { get; protected set; }
        public float Health { get; protected set; }

        float _lastDamaged = float.NegativeInfinity;
        static float s_DamageFlashTime = GameConstants.FIXED_DELTA_TIME;

        protected Vector2 _moveVector = Vector2.Zero;
        public float Speed { get; set; }

        public Unit(Vector2 size, Vector2 position, float health, float speed,  Brush brush) : base(size, position, brush)
        {
            Health = health;
            MaxHealth = health;
            Speed = speed;
            Weapon = NullWeapon.INSTANCE;
            _despawnDelay = 1f; // For units specifically.
        }


        public override void Update(float deltaTime, float currentTime)
        {

            Controller?.Update(this, deltaTime, currentTime);

            _position += _moveVector * deltaTime;
            _moveVector = Vector2.Zero;
        }

        internal void Move(Vector2 moveVector)
        {
            _moveVector = moveVector;
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

            // Bar dimensions
            int barHeight = 6;
            int barOffset = 4; // gap below entity rect
            int barSpacing = 2; // gap between bars

            // --- Health Bar ---
            Rectangle healthBarBackgroundRect = new Rectangle(
                entityRect.X,
                entityRect.Bottom + barOffset,
                entityRect.Width,
                barHeight);

            g.FillRectangle(Brushes.DarkGray, healthBarBackgroundRect);

            float healthRatio = (float)Health / MaxHealth;
            int healthFillWidth = (int)(entityRect.Width * healthRatio);

            Rectangle healthFillRect = new Rectangle(
                entityRect.X,
                entityRect.Bottom + barOffset,
                healthFillWidth,
                barHeight);

            g.FillRectangle(Brushes.Red, healthFillRect);

            // --- Mana Bar (below health) ---

            if (Weapon != null)
            {
                int manaBarY = entityRect.Bottom + barOffset + barHeight + barSpacing;

                Rectangle manaBarBackgroundRect = new Rectangle(
                    entityRect.X,
                    manaBarY,
                    entityRect.Width,
                    barHeight);

                g.FillRectangle(Brushes.DarkGray, manaBarBackgroundRect);

                float manaRatio = (float)Math.Min(1f, Weapon.GetTimeSinceLastShot(currentTime) / Weapon.GetCooldown());
                int manaFillWidth = (int)(entityRect.Width * manaRatio);

                Rectangle manaFillRect = new Rectangle(
                    entityRect.X,
                    manaBarY,
                    manaFillWidth,
                    barHeight);

                g.FillRectangle(Brushes.Blue, manaFillRect);
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
