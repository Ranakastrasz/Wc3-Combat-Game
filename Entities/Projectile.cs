using System.Collections.Immutable;
using System.Diagnostics;

using Microsoft.Xna.Framework;

using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Core.Event.Wc3_Combat_Game.Core.Event;
using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;

using static Wc3_Combat_Game.Core.GameConstants;

using Color = System.Drawing.Color;
using Vector2 = System.Numerics.Vector2;

namespace Wc3_Combat_Game.Entities
{
    /// <summary>
    /// Projectile object representing bullets, missiles, or other fired entities.
    /// Inherits from Entity.
    /// </summary>
    public class Projectile: Entity
    {
        //private Vector2 _velocity;
        private float _timeToLive;
        private ProjectilePrototype _prototype;
        public ImmutableArray<IGameplayAction> ImpactEffects => _prototype.ImpactActions;
        public Entity? Caster;
        private Team _team;

        private bool _hasImpactedThisTick = false;


        // Store delegates for clean unsubscription
        private BeforeCollisionEventHandler _beforeCollisionHandler;
        private OnCollisionEventHandler _onCollisionHandler;
        private AfterCollisionEventHandler _afterCollisionHandler;

        public Projectile(ProjectilePrototype prototype, Entity? caster, Vector2 position, Vector2 direction, IBoardContext context) : base(prototype.Radius, position, context)
        {
            _prototype = prototype;
            Caster = caster;
            _timeToLive = prototype.Lifespan;


            Func<IDrawContext, Color> getColor = (context) =>
            {

                if(IsAlive)
                {
                    return prototype.Color;
                }
                else
                {
                    return Color.Gray;
                }

            };
            Drawer = new PolygonDrawable((context) => getColor(context), () => Position, () => this.IsAlive? _prototype.Radius * 2:_prototype.Radius, () => _physicsBody.Body.Rotation, () => 1, () => true);
            _team = caster?.Team ?? Team.Neutral;

            _despawnDelay = 1f; // For units specifically.


            _physicsBody.Velocity = GeometryUtils.NormalizeAndScale(direction, prototype.Speed);


            //_physicsBody.Body.Rotation = MathF.Atan2(direction.Y, direction.X);
            // Just doing this broke collision entirely instead of partially.

            Body body = _physicsBody.Body;
            //body.FixedRotation = true;

            if(body.FixtureList.Count > 0)
            {
                Fixture fixture = body.FixtureList[0];
                fixture.CollisionCategories = PhysicsManager.Projectile;
                fixture.CollidesWith = PhysicsManager.Unit | PhysicsManager.Terrain;
                //fixture.IsSensor = true;
                body.IsBullet = true;
                body.LinearDamping = 0f;
                fixture.Friction = 0f;
                fixture.Restitution = 0.5f;
                _beforeCollisionHandler = (f1, f2) =>
                {
                    
                    if(!IsAlive || _hasImpactedThisTick) // If already "dead" or marked for impact this tick, skip further processing
                        return false; // skip collision if already dead
                    // No sure if _hasImpactedThisTick can even be true here, but let's be safe.

                    var myEntity = caster;
                    var otherObject = f2.Body.Tag;

                    if(otherObject is Entity otherEntity)
                    {
                        if(otherEntity.IsAlive == false)
                            return false; // skip collision if other entity is already dead
                        if(_team.IsHostileTo(otherEntity.Team))
                        {
                            return true; // allow collision with hostile entities
                        }
                        return false; // skip collision with non-hostile entities
                    }
                    else if(otherObject is Tile tile)
                    {
                        // I suspect I can ask the f2 fixture if it is "terrain" here instead.
                        return true; // allow collision with terrain
                    }
                    else
                    {
                        Debug.WriteLine($"Unknown object type in collision: {otherObject?.GetType().Name}");
                        return false;
                    }

                };
                _onCollisionHandler = (f1, f2, contact) =>
                {
                    if(!IsAlive || _hasImpactedThisTick) // Double check here too.
                        return false; // Skip if already handled or dead
                    var myEntity = caster;
                    var otherObject = f2.Body.Tag;


                    if(otherObject is Entity otherEntity)
                    {
                        if(otherEntity.IsAlive == false)
                            return false; // skip collision if other entity is already dead, because of a previous collision this tick.

                        //contact.Restitution = 0f;
                        contact.Enabled = false;

                        // Get the velocity of the projectile before the collision.
                        var projectileVelocity = f1.Body.LinearVelocity;

                        // Get the normal of the collision (direction of impact).
                        contact.GetWorldManifold(out Microsoft.Xna.Framework.Vector2 normal, out _);

                        // Calculate the "bounced" velocity. This is a manual implementation
                        // of a perfect bounce (reversing the velocity along the normal).
                        var reflectionVector = Microsoft.Xna.Framework.Vector2.Reflect(projectileVelocity, normal);

                        // Set the projectile's velocity to the calculated bounce vector.
                        f1.Body.LinearVelocity = reflectionVector;

                        // Mark for impact this tick
                        // Don't otherwise run any onCollision effects. That is for the AfterCollision.
                        _hasImpactedThisTick = true;


                        return true; // Allow collision to be resolved physically (bounce/ricochet)


                    }
                    else if(otherObject is Tile tile)
                    {
                        _hasImpactedThisTick = true;

                    }
                    return true; // allow it
                };

                // Post solve includes the Collision impulse and normals.

                _afterCollisionHandler = (f1, f2, contact, impulse) =>
                {
                    if(!IsAlive || !_hasImpactedThisTick) // Double check yet again.
                        return; // Skip if already handled or dead
                    var myEntity = caster;
                    var otherObject = f2.Body.Tag;
                    var targetId = otherObject is Entity e ? e.Index : -1;
                    // Override contact properties to prevent further interactions


                    // optional normal and impulse get. Not needed yet.
                    // contact.GetWorldManifold(...) etc.

                    contact.GetWorldManifold(out Microsoft.Xna.Framework.Vector2 normal, out FixedArray2<Microsoft.Xna.Framework.Vector2> points);

                    // Use the first contact point as the origin for the AOE
                    Microsoft.Xna.Framework.Vector2 impactPoint = points[0];

                   
                    // create the event and publish it
                    var impactEvent = new ProjectileImpactEvent(
                        projectileId: this.Index,
                        casterId: Caster?.Index ?? -1, // Use a default value if Caster is null
                        targetId: targetId,
                        impactPoint: impactPoint,
                        context: context
                    );
                    // Publish the event to notify other systems
                    context.EventBus.Publish(impactEvent);
                    
                    Die(context);

                    _hasImpactedThisTick = false;
                };

                fixture.BeforeCollision += _beforeCollisionHandler;
                fixture.OnCollision += _onCollisionHandler;
                fixture.AfterCollision += _afterCollisionHandler;
            }
            else
            {
                throw new Exception($"Projectile fixture for {prototype.Name} is null or empty. Ensure the prototype has a valid fixture.");
            }

        }

        public override void Update(float deltaTime, IBoardContext context)
        {
            base.Update(deltaTime, context);
            if(IsAlive)
            {

                _timeToLive -= FIXED_DELTA_TIME;
                if(_timeToLive <= 0)
                {
                    Die(context);
                }
            }
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            DrawDebug(g, context);
            base.Draw(g, context);

        }

        internal override void DrawDebug(Graphics g, IDrawContext context)
        {
            base.DrawDebug(g, context);

        }


        public override void Die(IBoardContext context)
        {
            if(!IsAlive) return;
            base.Die(context); // Call base implementation for any common Entity death logic. Declares the entity _isAlive = false

            // Apply visual lingering effects here, but disable physics interaction immediately

            _physicsBody.Body.LinearDamping = 10f; // Rapid deceleration for visual slow down and stop.
            _physicsBody.Body.IsBullet = false; // Disable bullet behavior because we no longer need high detail collision detection

            Fixture fixture = _physicsBody.Body.FixtureList[0];
            if(fixture != null)
            {
                // Crucial: Only interact with Terrain.
                fixture.CollidesWith = PhysicsManager.Terrain;
                fixture.CollisionCategories = PhysicsManager.Projectile;// | PhysicsManager.Dead;
                // I Might make a dead category. Not right now, since it would break other things though. I think.

                // Unsubscribe events once the projectile is truly dead and should no longer interact
                // This prevents zombie callbacks.
                fixture.OnCollision -= _onCollisionHandler;
                fixture.BeforeCollision -= _beforeCollisionHandler;
                fixture.AfterCollision -= _afterCollisionHandler;
            }

        }
    }
}
