using System.Numerics;
using Wc3_Combat_Game.Prototype;
using Wc3_Combat_Game.Util;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Components.Weapons;
using Wc3_Combat_Game.Components.Weapons.Interface;
using Wc3_Combat_Game.Components.Controllers.Interface;
using Wc3_Combat_Game.Components.Controllers;


namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Represents a living or interactive game unit with health and actions.
    /// Inherits from Entity.
    /// </summary>
    public class Unit : MobileEntity
    {

        public IUnitController? Controller = null;
        public Unit? Target { get; set; }

        public IWeapon? Weapon; // needs to be a weapon list instead.\

        public Vector2? TargetPoint { get; set; } = null; // For movement orders.
        public float MaxHealth { get; protected set; }
        public float Health { get; protected set; }

        float _lastDamaged = float.NegativeInfinity;
        static float s_DamageFlashTime = GameConstants.FIXED_DELTA_TIME;

        public float MoveSpeed { get; set; }

        public Unit(UnitPrototype prototype, Vector2 position) : base(prototype.Size, position, prototype.FillColor)
        {
            Health = prototype.MaxHealth;
            MaxHealth = prototype.MaxHealth;
            MoveSpeed = prototype.Speed;
            if (prototype.Weapon is WeaponPrototypeBasic basic)
            {
                Weapon = new BasicWeapon(basic);
            }
            _despawnDelay = 1f; // For units specifically.
        }


        public override void Update(float deltaTime, IBoardContext context)
        {
            if (deltaTime <= 0f) return; // No time has passed, no update needed.
            Controller?.Update(this, deltaTime, context);

            if(TargetPoint != null)
            {
                Vector2 moveVector = (Vector2)TargetPoint - Position;
                if(Vector2.DistanceSquared(Position, (Vector2)TargetPoint) < MoveSpeed*deltaTime)
                    _velocity = moveVector / deltaTime; // Just reach the point this frame.
                else
                    _velocity = GeometryUtils.NormalizeAndScale(moveVector, MoveSpeed);
            }



            base.Update(deltaTime, context); // Includes movement and collision.

            // Units only move once tick of movement per "move order",
            _velocity = Vector2.Zero;
        }



        public override void Draw(Graphics g, IDrawContext context)
        {
            Controller?.DrawDebug(g, context, this);

            RectangleF entityRect = _position.RectFFromCenter(_sizeVector);

            if (!TimeUtils.HasElapsed(context.CurrentTime,_lastDamaged,s_DamageFlashTime))
            {
                g.FillEllipse(Brushes.White, entityRect);
            }
            else if (IsAlive)
            {
                using var brush = new SolidBrush(_fillColor);
                g.FillEllipse(brush, entityRect);
            }
            else
            {
                g.FillEllipse(Brushes.Gray, entityRect);
            }

            // Bar dimensions
            int barHeight = 6;
            int barOffset = 4; // gap below entity rect
            int barSpacing = 2; // gap between bars

            if (Health < MaxHealth && Health > 0)
            {
                // --- Health Bar ---
                RectangleF healthBarBackgroundRect = new RectangleF(
                    entityRect.X,
                    entityRect.Bottom + barOffset,
                    entityRect.Width,
                    barHeight);

                g.FillRectangle(Brushes.DarkGray, healthBarBackgroundRect);

                float healthRatio = (float)Health / MaxHealth;
                int healthFillWidth = (int)(entityRect.Width * healthRatio);

                RectangleF healthFillRect = new RectangleF(
                    entityRect.X,
                    entityRect.Bottom + barOffset,
                    healthFillWidth,
                    barHeight);

                g.FillRectangle(Brushes.Red, healthFillRect);

            }
            // --- Mana Bar (below health) ---
            // --- More of a cooldown bar. For player only.
            if (Team == TeamType.Ally)
            {
                if (Weapon != null)
                {
                    float manaBarY = entityRect.Bottom + barOffset + barHeight + barSpacing;

                    RectangleF manaBarBackgroundRect = new RectangleF(
                        entityRect.X,
                        manaBarY,
                        entityRect.Width,
                        barHeight);

                    g.FillRectangle(Brushes.DarkGray, manaBarBackgroundRect);

                    float manaRatio = (float)Math.Min(1f, Weapon.GetTimeSinceLastShot(context) / Weapon.GetCooldown());
                    int manaFillWidth = (int)(entityRect.Width * manaRatio);

                    RectangleF manaFillRect = new RectangleF(
                        entityRect.X,
                        manaBarY,
                        manaFillWidth,
                        barHeight);

                    g.FillRectangle(Brushes.Blue, manaFillRect);
                }
            }
        }

        public void Damage(float amount, IBoardContext context)
        {
            Health -= amount;
            _lastDamaged = context.CurrentTime;
            if (Health <= 0f)
            {
                Health = 0;
                Die(context);
            }
        }
    }
    static class UnitFactory
    {
        public static Unit SpawnUnit(UnitPrototype prototype, Vector2 position, IUnitController controller, TeamType team)
        {
            Unit unit = new Unit(prototype, position)
            {
                Controller = controller,
                Team = team
            };
            return unit;
        }
    }
}
