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
    internal class Player : Unit
    {
        private static readonly float MAX_HEALTH = 100f;

        static readonly float COOLDOWN = 0.35f;
        float _lastShotTime = float.NegativeInfinity;

        

        public Player(Vector2 size, Vector2 position, Brush brush) : base(size, position, brush)
        {
            Health = MAX_HEALTH;
        }

        internal void InputMove(Vector2 move)
        {
            _moveVector = move;
        }

        // Horrifically temperary, because passing that container is VERY wrong.
        public bool TryShoot(Vector2 target, float currentTime, EntityManager<Projectile> Projectiles)
        {


            if (currentTime >= (_lastShotTime + COOLDOWN))
            {
                Vector2 velocity = Vector2.Normalize(target - this.Position) * PROJECTILE_SPEED;


                Projectile projectile = new((Vector2)PROJECTILE_SIZE,
                    this.Position,
                    PROJECTILE_COLOR,
                    velocity,
                    PROJECTILE_LIFESPAN);

                // add to the list
                // This should be a function. Intrusive.
                Projectiles.Add(projectile);

                _lastShotTime = currentTime; // Set Cooldown.
                return true;
            }
            return false;
        }


        public override void Update(float deltaTime, float currentTime)
        {
            if (!IsAlive) return;
            _position += _moveVector * FIXED_DELTA_TIME;
            _moveVector = Vector2.Zero; // No inertia for now

            //_health += Math.Min(1f * FIXED_DELTA_TIME, _maxhealth); // Regen
        }

        public override void Draw(Graphics g, float currentTime)
        {
            //if (!IsAlive) return;

            base.Draw(g, currentTime);
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

            float healthRatio = (float)Health / MAX_HEALTH;
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

            float manaRatio = (float)Math.Min(1f, (currentTime - _lastShotTime) / COOLDOWN);
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
