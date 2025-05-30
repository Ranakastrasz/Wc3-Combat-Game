using System.Numerics;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Util;
using static Wc3_Combat_Game.GameConstants;

namespace Wc3_Combat_Game
{
    /// <summary>
    /// Player-controlled unit with input handling and player-specific behavior.
    /// Inherits from Unit.
    /// </summary>
    public class Player : Unit
    {
        Vector2 _velocity = Vector2.Zero;
        private static readonly float MAX_HEALTH = 100f;

        static readonly float COOLDOWN = 0.35f;
        float _lastShot = 0f;

        

        public Player(Vector2 size, Vector2 position, Brush brush) : base(size, position, brush)
        {
            _health = MAX_HEALTH;
        }

        internal void InputMove(Vector2 move)
        {
            _velocity = move;
        }

        public bool TryShoot(Vector2 target)
        {
            if (GameManager.Instance.GlobalTime >= (_lastShot + COOLDOWN))
            {
                Vector2 velocity = Vector2.Normalize(target - this.Position) * PROJECTILE_SPEED;


                Projectile projectile = new((Vector2)PROJECTILE_SIZE,
                    this.Position,
                    PROJECTILE_COLOR,
                    velocity,
                    PROJECTILE_LIFESPAN);

                // add to the list
                // This should be a function. Intrusive.
                GameManager.Instance.Projectiles.Add(projectile);

                _lastShot = GameManager.Instance.GlobalTime; // Set Cooldown.
                return true;
            }
            return false;
        }


        public override void Update()
        {
            _position += _velocity * FIXED_DELTA_TIME;
            _velocity = Vector2.Zero; // No inertia for now

            //_health += Math.Min(1f * FIXED_DELTA_TIME, _maxhealth); // Regen
        }

        public override void Draw(Graphics g)
        {
            //if (!IsAlive) return;

            base.Draw(g);
            Rectangle entityRect = _position.RectFromCenter(_size);

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

            float healthRatio = (float)_health / MAX_HEALTH;
            int healthFillWidth = (int)(entityRect.Width * healthRatio);

            Rectangle healthFillRect = new Rectangle(
                entityRect.X,
                entityRect.Bottom + barOffset,
                healthFillWidth,
                barHeight);

            g.FillRectangle(Brushes.Red, healthFillRect);

            // --- Mana Bar (below health) ---
            int manaBarY = entityRect.Bottom + barOffset + barHeight + barSpacing;

            Rectangle manaBarBackgroundRect = new Rectangle(
                entityRect.X,
                manaBarY,
                entityRect.Width,
                barHeight);

            g.FillRectangle(Brushes.DarkGray, manaBarBackgroundRect);

            float manaRatio = (float)Math.Min(1f, (GameManager.Instance.GlobalTime - _lastShot) / COOLDOWN);
            int manaFillWidth = (int)(entityRect.Width * manaRatio);

            Rectangle manaFillRect = new Rectangle(
                entityRect.X,
                manaBarY,
                manaFillWidth,
                barHeight);

            g.FillRectangle(Brushes.Blue, manaFillRect);


        }
    }
}
