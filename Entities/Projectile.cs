using Microsoft.Xna.Framework;

using nkast.Aether.Physics2D.Common;
using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
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
        public IGameplayAction? ImpactEffect => _prototype.ImpactEffect;
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
            Drawer = new PolygonDrawable((context) => getColor(context), () => Position, () => this.IsAlive? _prototype.Radius * 2:_prototype.Radius, () => 1, () => true);
            _team = caster?.Team ?? Team.Neutral;

            _despawnDelay = 1f; // For units specifically.


            _physicsObject.Velocity = GeometryUtils.NormalizeAndScale(direction, prototype.Speed);
            Body body = _physicsObject.Body;
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
                    var myEntity = caster;
                    var otherObject = f2.Body.Tag;
                    if(otherObject is Entity otherEntity)
                    {
                        if(otherEntity.IsAlive == false)
                            return false; // skip collision if other entity is already dead
                        if(_team.IsHostileTo(otherEntity.Team))
                        {
                            //Console.WriteLine("----BeforeCollision Debug----");
                            //Console.WriteLine($"Projectile ({f1.Body.Tag}) vs Other ({f2.Body.Tag})");
                            //Console.WriteLine($"IsAlive (Projectile): {IsAlive}, IsAlive (Other): {(f2.Body.Tag as //Entity)?.IsAlive}");
                            //Console.WriteLine($"Projectile Team: {_team}, Other Team: {(f2.Body.Tag as Entity)?.Team}");
                            //Console.WriteLine($"IsHostile: {_team.IsHostileTo((f2.Body.Tag as Entity)?.Team ?? Team.Neutral)}");
                            //Console.WriteLine($"Projectile IsSensor: {f1.IsSensor}, Other IsSensor: {f2.IsSensor}");
                            //
                            //
                            //Console.WriteLine($"Projectile Velocity: {f1.Body.LinearVelocity}");
                            //Console.WriteLine($"Other Body Velocity: {f2.Body.LinearVelocity}");
                            //Console.WriteLine($"Projectile Mass: {f1.Body.Mass}");
                            //Console.WriteLine($"Other Body Mass: {f2.Body.Mass}");
                            //Console.WriteLine($"Projectile Restitution: {f1.Restitution}");
                            //Console.WriteLine($"Other Body Restitution: {f2.Restitution}");
                            //Console.WriteLine($"--- END BeforeCollision DEBUG ---");

                            return true; // allow collision with hostile entities
                        }
                        return false; // skip collision with non-hostile entities
                    }
                    else if(otherObject is Tile tile)
                    {
                        return true; // allow collision with terrain
                    }
                    else
                    {
                        throw new MissingFieldException($"Unknown object type in collision: {otherObject?.GetType().Name}");
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
                            return false; // skip collision if other entity is already dead

                        /*Microsoft.Xna.Framework.Vector2 worldNormal = new Microsoft.Xna.Framework.Vector2();
                        var worldPoints = new FixedArray2<Microsoft.Xna.Framework.Vector2>();
                        
                        
                        contact.GetWorldManifold(out worldNormal, out worldPoints);
                        
                        Console.WriteLine($"--- OnCollision DEBUG ---");
                        Console.WriteLine($"  *** WORLD MANIFOLD DATA ***");
                        Console.WriteLine($"  World Normal: {worldNormal}");
                        Console.WriteLine($"  World Point 0: {worldPoints[0]}");
                        
                        Console.WriteLine($"  NormalImpulse: {contact.Manifold.Points[0].NormalImpulse}");
                        Console.WriteLine($"  TangentImpulse: {contact.Manifold.Points[0].TangentImpulse}");
                        Console.WriteLine($"  Normal: {contact.Manifold.LocalNormal}");
                        
                        Console.WriteLine($"Projectile ({f1.Body.Tag}) vs Other ({f2.Body.Tag})");
                        Console.WriteLine($"IsAlive (Projectile): {IsAlive}, IsAlive (Other): {(f2.Body.Tag as Entity)?.IsAlive}");
                        Console.WriteLine($"Projectile Team: {_team}, Other Team: {(f2.Body.Tag as Entity)?.Team}");
                        Console.WriteLine($"IsHostile: {_team.IsHostileTo((f2.Body.Tag as Entity)?.Team ?? Team.Neutral)}");
                        Console.WriteLine($"Projectile IsSensor: {f1.IsSensor}, Other IsSensor: {f2.IsSensor}");
                        Console.WriteLine($"Projectile CollisionCategories: {f1.CollisionCategories}, CollisionGroup: /{f1.CollisionGroup}, CollidesWith: {f1.CollidesWith}");
                        Console.WriteLine($"Other CollisionCategories: {f2.CollisionCategories}, CollisionGroup: {f2./CollisionGroup}, CollidesWith: {f2.CollidesWith}");
                        
                        Console.WriteLine($"Contact Enabled: {contact.Enabled}");
                        Console.WriteLine($"Contact IsTouching: {contact.IsTouching}");
                        Console.WriteLine($"Contact Point Count: {contact.Manifold.PointCount}");
                        
                        if(contact.Manifold.PointCount > 0)
                        {
                            for(int i = 0; i < contact.Manifold.PointCount; i++)
                            {
                                Console.WriteLine($"  Point {i}: Local={contact.Manifold.Points[i].LocalPoint}, NormalImpulse=/{contact.Manifold.Points[i].NormalImpulse}, TangentImpulse={contact.Manifold.Points/[i].TangentImpulse}");
                                Console.WriteLine($"  World Normal: {contact.Manifold.LocalNormal}");
                            }
                        }
                        else
                        {
                            Console.WriteLine("  No contact points reported in manifold.");
                        }
                        
                        Console.WriteLine($"Projectile Velocity: {f1.Body.LinearVelocity}");
                        Console.WriteLine($"Other Body Velocity: {f2.Body.LinearVelocity}");
                        Console.WriteLine($"Projectile Mass: {f1.Body.Mass}");
                        Console.WriteLine($"Other Body Mass: {f2.Body.Mass}");
                        Console.WriteLine($"Projectile Restitution: {f1.Restitution}");
                        Console.WriteLine($"Other Body Restitution: {f2.Restitution}");
                        Console.WriteLine($"Projectile BodyType: {f1.Body.BodyType}");
                        Console.WriteLine($"Other BodyType: {f2.Body.BodyType}");
                        Console.WriteLine($"Projectile Mass (re-read): {f1.Body.Mass}");
                        Console.WriteLine($"Other Mass (re-read): {f2.Body.Mass}");
                        Console.WriteLine($"--- END OnCollision DEBUG ---");*/

                        // Mark for impact this tick
                        _hasImpactedThisTick = true;
                        OnEntityCollision(otherEntity, context); // Apply immediate gameplay effects (damage, etc.)
                        Die(context);

                        // --- END Debug --
                        return true; // Allow collision to be resolved physically (bounce/ricochet)


                    }
                    else if(otherObject is Tile tile)
                    {
                        _hasImpactedThisTick = true;
                        OnTerrainCollision(context);

                        Die(context);
                    }
                    return true; // allow it
                };

                // Post solve includes the Collision impulse and normals.
                // May be needed for some effect calculations.
                _afterCollisionHandler = (f1, f2, contact, impulse) =>
                {
                    // Ensure the projectile *was* alive and impacted this tick.
                    // The _hasImpactedThisTick flag set in OnCollision is good for this.
                    if(!_hasImpactedThisTick)
                        return; // This collision was not meant to result in an "impact" for this projectile.

                    /*contact.GetWorldManifold(
                        out Microsoft.Xna.Framework.Vector2 worldNormal,
                        out FixedArray2<Microsoft.Xna.Framework.Vector2> worldPoints);

                    // Use the projectile's velocity *at the moment of collision*.
                    // f1.Body.LinearVelocity is correct for this.
                    Microsoft.Xna.Framework.Vector2 projectileVelocityAtImpact = f1.Body.LinearVelocity;

                    // Normalize both vectors
                    Microsoft.Xna.Framework.Vector2 normalizedProjectileVelocity = projectileVelocityAtImpact;
                    if(normalizedProjectileVelocity.LengthSquared() > 0)
                        normalizedProjectileVelocity.Normalize();

                    Microsoft.Xna.Framework.Vector2 normalizedWorldNormal = worldNormal;
                    if(normalizedWorldNormal.LengthSquared() > 0)
                        normalizedWorldNormal.Normalize();

                    // --- Step 1: Detect "Wrong Side" / Unintended Impact Direction ---
                    // The normal points from f1 (projectile) to f2 (target).
                    // For a 'front' hit, projectile's velocity should be generally *opposite* to the normal.
                    // So, the dot product (velocity . normal) should be negative.
                    // If it's positive, the projectile is moving along the normal, indicating a "wrong side" hit.
                    float velocityNormalDotProduct = Microsoft.Xna.Framework.Vector2.Dot(normalizedProjectileVelocity, normalizedWorldNormal);

                    bool isWrongSideImpact = false;
                    // A small positive threshold. If dot product is significantly positive, it's a wrong-side hit.
                    if(velocityNormalDotProduct > 0.05f) // Increased threshold slightly for robustness
                    {
                        isWrongSideImpact = true;
                        Console.WriteLine($"WARNING: Projectile might have impacted on the 'wrong side' or at an unusual angle! Dot Product: {velocityNormalDotProduct}");
                    }


                    // --- Step 2: Determine Collision Angle (Straight On vs. Glancing) ---
                    // The angle between the projectile's incoming velocity and the *inverse* collision normal.
                    // The inverse normal (-normalizedWorldNormal) points *into* the object that was hit.
                    // A dot product of 1 means direct head-on (angle 0).
                    // A dot product of 0 means perfectly glancing (angle 90).
                    // A dot product of -1 means moving directly away from the normal (angle 180 - not typically an incoming hit).
                    float impactDotProduct = Microsoft.Xna.Framework.Vector2.Dot(normalizedProjectileVelocity, -normalizedWorldNormal);

                    // Clamp the dot product to the valid range [-1, 1] for Acos, to prevent NaN issues with floating point inaccuracies.
                    impactDotProduct = MathHelper.Clamp(impactDotProduct, -1f, 1f);

                    float angleBetweenProjectileVelAndInverseNormal = (float)Math.Acos(impactDotProduct);
                    float angleInDegrees = MathHelper.ToDegrees(angleBetweenProjectileVelAndInverseNormal);

                    string collisionAngleDescription;
                    float straightOnThresholdDegrees = 30f; // More conservative for "straight on"
                    float glancingThresholdDegrees = 60f; // For "glancing" impacts

                    if(angleInDegrees <= straightOnThresholdDegrees)
                    {
                        collisionAngleDescription = "Straight On (Direct)";
                    }
                    else if(angleInDegrees >= glancingThresholdDegrees)
                    {
                        // This means the angle is closer to 90 degrees (perpendicular to inverse normal), so it's glancing.
                        collisionAngleDescription = "Glancing";
                    }
                    else
                    {
                        collisionAngleDescription = "Moderate Angle";
                    }

                    Console.WriteLine($"Collision Angle: {collisionAngleDescription} ({angleInDegrees:F2} degrees)");


                    // --- Step 3: Combine for Final Classification---
                    string finalCollisionType;
                    if(isWrongSideImpact)
                    {
                        finalCollisionType = "Phased / Backside Impact (Likely not a true hit)";
                    }
                    else
                    {
                        finalCollisionType = collisionAngleDescription; // Use the angle description if not wrong side
                    }

                    Console.WriteLine($"Final Collision Classification: {finalCollisionType}");*/

                    // Reset the flag for the next tick
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

        private void OnEntityCollision(Entity otherEntity, IBoardContext context)
        {
            if(_team.IsHostileTo(otherEntity.Team))
            {
                ImpactEffect?.ExecuteOnEntity(Caster, this, otherEntity, context);
            }
        }
        public override void OnTerrainCollision(IBoardContext context)
        {
            ImpactEffect?.ExecuteOnPoint(Caster, this, Position, context);
        }


        public override void Die(IBoardContext context)
        {
            if(!IsAlive) return;

            // Apply visual lingering effects here, but disable physics interaction immediately
            _physicsObject.Body.LinearDamping = 10f; // Rapid deceleration for visual "stop"

            Fixture fixture = _physicsObject.Body.FixtureList[0];
            if(fixture != null)
            {
                // Crucial: Disable further collision detection
                fixture.CollidesWith = PhysicsManager.Terrain;
                fixture.CollisionCategories = PhysicsManager.Projectile | PhysicsManager.Dead;

                // Unsubscribe events once the projectile is truly dead and should no longer interact
                // This prevents zombie callbacks.
                fixture.OnCollision -= fixture.OnCollision; // Note: You might need to store the actual delegates
                fixture.BeforeCollision -= fixture.BeforeCollision; // if you used `+= new CollisionHandler(...)`
                fixture.AfterCollision -= fixture.AfterCollision; // Unsubscribe this too
            }

            base.Die(context); // Call base implementation for any common Entity death logic. Declares the entity _isAlive = false
        }
    }
}
