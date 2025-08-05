using System.Numerics;

using AssertUtils;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Abilities;
using Wc3_Combat_Game.Entities.Components.Collider;
using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Movement;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.Util;


namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Represents a living or interactive game unit with health and actions.
    /// Inherits from Entity.
    /// </summary>
    public class Unit: Entity
    {
        protected UnitPrototype _prototype;

        public IUnitController? Controller = null;
        public Unit? TargetUnit { get; set; }

        public List<IAbility> Abilities = new();

        public Vector2? TargetPoint
        {
            get => _targetPoint;
            set => _targetPoint = value;
            //{
            //    AssertUtil.Assert(() => value == null || !value.Value.IsNaN());
            //    _targetPoint = value;
            //}
        }

        public Vector2? GetTargetPosition()
        {
            if(TargetUnit != null && TargetUnit.IsAlive)
                return TargetUnit.Position;
            return TargetPoint;
        }
        public float MaxLife => _prototype.MaxLife;
        public float Life { get; protected set; }

        public float LifeRegen => _prototype.LifeRegen;

        public float MaxMana => _prototype.MaxMana;
        public float Mana { get; set; }
        public float ManaRegen => _prototype.ManaRegen;

        float _lastDamaged = float.NegativeInfinity;
        static float s_DamageFlashTime = GameConstants.FIXED_DELTA_TIME;

        private Vector2? _targetPoint = null;

        public float MoveSpeed { get; set; }
        public float SlowExpires { get; internal set; }

        //public new IMoveable Mover { get;}

        public Unit(UnitPrototype prototype, Vector2 position, IBoardContext context) : base(prototype.Radius, position, context)
        {
            _prototype = prototype;
            Life = prototype.MaxLife;
            Mana = prototype.MaxMana;
            MoveSpeed = prototype.Speed;

            for(int x = 0; x < _prototype.Abilities.Length; x++)
            {
                if(prototype.Abilities[x] is TargetedAbilityPrototype basic)
                {
                    Abilities.Add(new TargetedAbility(basic));
                }
            }
            _despawnDelay = 1f;

            Func<IDrawContext, Color> getColor = (context) =>
            {
                
                if(!TimeUtils.HasElapsed(context.CurrentTime, _lastDamaged, s_DamageFlashTime))
                {
                    return prototype.DamagedColor;
                }
                else if(IsAlive)
                {
                    return prototype.Color;
                }
                else
                {
                    return Color.Gray;
                }

            };
            Drawer = new PolygonDrawable(getColor, () => Position, () => Radius * 2, () => prototype.PolygonCount , () => true);

            Body body = _physicsObject.Body;
            if(body.FixtureList.Count > 0)
            {
                Fixture fixture = body.FixtureList[0];
                fixture.CollisionCategories = PhysicsManager.Player | PhysicsManager.Unit;
                fixture.CollidesWith = PhysicsManager.Terrain | PhysicsManager.Unit | PhysicsManager.Projectile;
                //fixture.IsSensor = false;
                body.LinearDamping = 2f;
                fixture.Friction = 0f;
                fixture.Restitution = 0f; // unless bounce is needed
            }

        }


        public override void Update(float deltaTime, IBoardContext context)
        {
            if(deltaTime <= 0f) return; // No time has passed, no update needed.

            if(!IsAlive)
            {
                //    MoveSpeed *= 0.95f;
                base.Update(deltaTime, context); // Includes movement and collision.
                return;
            }
            if(context.Map?[context.Map.ToGrid(Position)].GetChar == 'F')
            {
                // If on a fountain tile, regenerate health and mana faster.
                Life = MathF.Min(Life + LifeRegen * 10f * deltaTime, MaxLife);
                Mana = MathF.Min(Mana + ManaRegen * 10f * deltaTime, MaxMana);
            }
            else
            {
                Life = MathF.Min(Life + LifeRegen * deltaTime, MaxLife);
                Mana = MathF.Min(Mana + ManaRegen * deltaTime, MaxMana);
            }

            Controller?.Update(this, deltaTime, context);

            if(TargetPoint != null)
            {
                Vector2 moveVector = (Vector2)TargetPoint - Position;
                if(Vector2.DistanceSquared(Position, (Vector2)TargetPoint) < MoveSpeed * deltaTime)
                    _physicsObject.Velocity = moveVector / deltaTime; // Just reach the point this frame.
                else
                    _physicsObject.Velocity = GeometryUtils.NormalizeAndScale(moveVector, MoveSpeed);
            }
            if(Abilities[0].OnCooldown(context.CurrentTime))
            {
                _physicsObject.Velocity *= 0.5f; // Slow down while shooting.
            }
            if(!TimeUtils.HasElapsed(context.CurrentTime, SlowExpires, 0f))
            {
                _physicsObject.Velocity *= 0.5f;
            }
            /*
             if(TargetPoint != null)
            {
                Vector2 moveVector = (Vector2)TargetPoint - Position;
                if(Vector2.DistanceSquared(Position, (Vector2)TargetPoint) < MoveSpeed * deltaTime)
                    _physicsObject.Velocity = moveVector / deltaTime; // Just reach the point this frame.
                else
                    _physicsObject.Velocity = GeometryUtils.NormalizeAndScale(moveVector, MoveSpeed);
            }
            if(Abilities[0].OnCooldown(context.CurrentTime))
            {
                _physicsObject.Velocity *= 0.5f; // Slow down while shooting.
            }
            if(!TimeUtils.HasElapsed(context.CurrentTime, SlowExpires, 0f))
            {
                _physicsObject.Velocity *= 0.5f;
            }
            //}
            base.Update(deltaTime, context); // Includes movement and collision.

            // Units only move once tick of movement per "move order",
            //_velocity = Vector2.Zero;
            }
             */

            base.Update(deltaTime, context); // Includes movement and collision.

            // Units only move once tick of movement per "move order",
            //_velocity = Vector2.Zero;
        }



        public override void Draw(Graphics g, IDrawContext context)
        {
            base.Draw(g, context);
            //base.DrawDebug(g, context);
            Controller?.DrawDebug(g, context, this);

            //if(!TimeUtils.HasElapsed(context.CurrentTime, _lastDamaged, s_DamageFlashTime))
            //{
            //    g.FillEllipse(Brushes.White, BoundingBox);
            //}
            //else if(IsAlive)
            //{
            //    var brush = context.DrawCache.GetSolidBrush(_fillColor);
            //    g.FillEllipse(brush, BoundingBox);
            //}
            //else
            //{
            //    g.FillEllipse(Brushes.Gray, BoundingBox);
            //}

            // Bar dimensions
            int barHeight = 2;
            int barOffset = 4; // gap below entity rect
            int barSpacing = 2; // gap between bars

            // Colors are bad, but worry later.
            // blends into the existing terrain.

            if(Life < MaxLife && Life > 0)
            {
                // --- Health Bar ---
                RectangleF healthBarBackgroundRect = new RectangleF(
                    BoundingBox.X,
                    BoundingBox.Bottom + barOffset,
                    BoundingBox.Width,
                    barHeight);

                g.FillRectangle(Brushes.DarkGray, healthBarBackgroundRect);

                float healthRatio = (float)Life / MaxLife;
                int healthFillWidth = (int)(BoundingBox.Width * healthRatio);

                RectangleF healthFillRect = new RectangleF(
                    BoundingBox.X,
                    BoundingBox.Bottom + barOffset,
                    healthFillWidth,
                    barHeight);

                g.FillRectangle(Brushes.Red, healthFillRect);

            }
            // --- Mana Bar (below health) ---
            // --- More of a cooldown bar. For player only.
            if(Team == Team.Ally)
            {
                if(Abilities != null)
                {
                    float manaBarY = BoundingBox.Bottom + barOffset + barHeight + barSpacing;

                    RectangleF manaBarBackgroundRect = new RectangleF(
                        BoundingBox.X,
                        manaBarY,
                        BoundingBox.Width,
                        barHeight);

                    g.FillRectangle(Brushes.DarkGray, manaBarBackgroundRect);

                    float manaRatio;

                    //if(Weapon is BasicWeapon basicWeapon)
                    //{
                    //// Calculate mana ratio based on weapon cooldown and time since last shot
                    //// This is a temporary solution, will be replaced with a proper mana system later.
                    //
                    //float manaCost = (basicWeapon.GetPrototype() as WeaponPrototypeBasic).ManaCost; // Hacky, but should work.
                    //int shotsLeft = (int)MathF.Floor(Mana / manaCost); // How many shots can be fired with current mana.
                    //
                    //manaRatio = MaxMana > 0 ? (float)Mana / MaxMana : 0f; // Avoid division by zero
                    //if(basicWeapon.Cooldown > 0f)
                    //    manaRatio = MathF.Min(1f, Weapon.GetTimeSinceLastShot(context) / basicWeapon.Cooldown);
                    //}
                    //else
                    if(MaxMana > 0f)
                    {
                        // Calculate mana ratio based on current mana and max mana
                        manaRatio = Mana / MaxMana;
                        // Ideally, needs to sort of round to the nearest shot that can be fired.
                    }
                    else
                    {
                        // If no mana system is implemented, use cooldown as a proxy for mana
                        // This is a temporary solution, will be replaced with a proper mana system later.

                        {
                            manaRatio = (float)Math.Min(1f, Abilities[0].GetTimeSinceLastUse(context) / Abilities[0].Cooldown);
                        }

                    }

                    int manaFillWidth = (int)(BoundingBox.Width * manaRatio);

                    RectangleF manaFillRect = new RectangleF(
                        BoundingBox.X,
                        manaBarY,
                        manaFillWidth,
                        barHeight);

                    g.FillRectangle(Brushes.Blue, manaFillRect);
                }
            }
        }

        public void Damage(float amount, IBoardContext context)
        {
            Life -= amount;
            _lastDamaged = context.CurrentTime;
            if(Life <= 0f)
            {
                Life = 0;
                Die(context);
            }
        }

        public bool HasClearPathTo(Vector2 targetPosition, IBoardContext context)
        {
            AssertUtil.NotNull(context.Map);
            if(context.Map.HasLineOfSight(Position, targetPosition, Radius))
            {
                return true;

            }
            return false;
        }

        public override void Die(IBoardContext context)
        {
            base.Die(context);
        }
    }
    static class UnitFactory
    {
        public static Unit SpawnUnit(UnitPrototype prototype, Vector2 position, IUnitController controller, Team team, IBoardContext context)
        {
            Unit unit = new Unit(prototype, position, context)
            {
                Controller = controller,
                Team = team
            };
            return unit;
        }
    }
}
