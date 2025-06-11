
using AssertUtils;
using System.ComponentModel;
using System.Windows.Forms;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Interface.Weapons;
using Wc3_Combat_Game.IO.Load.GameSchema;
using Wc3_Combat_Game.Prototype;

namespace Wc3_Combat_Game.Prototypes
{
    public static class EntityPrototyper
    {
        

            //var meleeWeaponBase = new PrototypeWeaponBasic(new Effects.Action(), 1f, 20f);
            //var weapon5Damage = meleeWeaponBase.SetDamage(5f);
            //var weapon10Damage = meleeWeaponBase.SetDamage(10f);
            //var weapon25Damage = meleeWeaponBase.SetDamage(25f);
            //var weapon200Damage = meleeWeaponBase.SetDamage(200f);
            //
            //var rangedWeaponBase = new PrototypeWeaponBasic(
            //    new ActionProjectile(new PrototypeProjectile(5, 225f, 4f, null, Color.Cyan)),
            //    1f,
            //    150f);
            //
            //var weapon10DamageRanged = rangedWeaponBase.SetDamage(10f);
            //
            //_waves.Add(new Wave(new PrototypeUnit(weapon5Damage       , 10f,   2f,  8f,  75f, Color.Brown  , PrototypeUnit.DrawShape.Circle), 16));
            //_waves.Add(new Wave(new PrototypeUnit(weapon10Damage      , 20f, 0.1f, 12f, 100f, Color.Red    , PrototypeUnit.DrawShape.Circle), 16));
            //_waves.Add(new Wave(new PrototypeUnit(weapon10DamageRanged, 30f, 0.1f, 10f,  50f, Color.Orange , PrototypeUnit.DrawShape.Square), 8));
            //_waves.Add(new Wave(new PrototypeUnit(weapon25Damage      , 80f,   2f, 20f,  75f, Color.Red    , PrototypeUnit.DrawShape.Square), 4));
            //_waves.Add(new Wave(new PrototypeUnit(weapon200Damage     , 400f,  0f, 30f, 125f, Color.DarkRed, PrototypeUnit.DrawShape.Square), 1));

        public static Dictionary<string, PrototypeProjectile> InitProjectileStubs(EnemyComponentSchema components)
        { 
            AssertUtil.AssertNotNull(components.Projectiles, "Projectiles not found in enemy schema");
            Dictionary<string, ProjectileSchema> projectileSchemas = components.Projectiles;
            AssertUtil.AssertNotNull(components.VisualEffects, "VisualEffects not found in enemy schema.");
            Dictionary<string, DrawableSchema> drawableSchemas = components.VisualEffects;

            Dictionary<string, PrototypeProjectile> projectiles = new Dictionary<string, PrototypeProjectile>();

            foreach (string key in projectileSchemas.Keys)
            {
                ProjectileSchema schema = projectileSchemas[key]; 
                if (schema == null)
                {
                    Console.WriteLine($"Warning: projectile with name '{key}' not found in schema.");
                    continue;
                }
                ActionDamage damageEffect = new ActionDamage(0f); // Stub

                PrototypeProjectile projectile = new PrototypeProjectile(
                    schema.VisualEffect?.EffectShapeSize ?? 1f,
                    schema.Speed,
                    float.PositiveInfinity, // Lifespan is infinite for now, can be changed later
                    damageEffect,
                    Color.FromName(schema.VisualEffect?.EffectColor ?? "Black")); // use color or default
                
                projectiles.Add(key,projectile);
            }

            return projectiles;
        }
        
        private static Dictionary<string, PrototypeUnit> InitPrototypeUnit(List<EntitySchema> entities, Dictionary<string, PrototypeProjectile> projectiles, Dictionary<string, DrawableSchema> visualEffects)
        {
            // Ok. I have the projectiles, now I need to create the units.
            // Each unit has a weapon, stats, and a drawable shape.
            Dictionary<string, PrototypeUnit> units = new Dictionary<string, PrototypeUnit>();
            foreach (string Key in entities.Select(e => e.Name).Where(n => n != null))
            {
                EntitySchema entitySchema = entities.FirstOrDefault(e => e.Name == Key);
                if (entitySchema == null)
                {
                    Console.WriteLine($"Warning: Entity with name '{Key}' not found in schema.");
                    continue;
                }

                AssertUtil.AssertNotNull(entitySchema.MeleeAttack);
                MeleeAttackSchema meleeAttack = entitySchema.MeleeAttack;
                AssertUtil.AssertNotNull(entitySchema.RangedAttacks?[0]);
                RangedAttackSchema rangedAttack = entitySchema.RangedAttacks[0]; // theoretically, there could be multiple ranged attacks, but for now we take the first one

                // now we have the melee and ranged attacks, we can create the weapons
                
                var meleeWeapon = new PrototypeWeaponBasic(
                    new ActionDamage(meleeAttack.DamageValue),
                    1f, // currently hardcoded attack speed, can be changed later
                    meleeAttack.Range); // Default to 100f if not specified

                PrototypeWeaponBasic? rangedWeapon = null;
                // If ranged attack is null, we can skip it
                if(rangedAttack != null)
                {
                    // Actually need to create a copy of the projectile stub, because they can have different stats.
                    var projectile = projectiles[rangedAttack.Actions[0].RefId]; // Assuming the first action is always a projectile_ref. Safe for now

                    rangedWeapon = new PrototypeWeaponBasic(
                        new ActionProjectile(PrototypeProjectile.Clone(projectile)), // Assuming Stats has a ProjectileRef
                        rangedAttack.Cooldown,
                        rangedAttack.Range);

                    rangedWeapon = rangedWeapon.SetDamage(rangedAttack.DamageValue); // Set the damage for the ranged weapon

                }

                var weapon = rangedWeapon ?? meleeWeapon; // If ranged is null, use melee weapon

                // Should have both melee and ranged weapons now, if ranged is not null

                PrototypeUnit unit = new PrototypeUnit(
                    weapon,
                    entitySchema.Stats.Health,
                    0f, // health regen is not used yet, so set to 0
                    entitySchema.Stats.Size,
                    entitySchema.Stats.Speed,
                    Color.FromName(entitySchema.Color), // Use the color from the schema
                    PrototypeUnit.DrawShape.Circle); // Default shape, can be changed later

                // finally, add the unit to the dictionary
                units.Add(Key, unit);
            }
            return units;
        }
        public static Dictionary<string, PrototypeUnit> InitEnemies(EnemySchema enemySchema)
        {
            // Ok I need to take the enemyScheme, have the schema be turned into the prototype entities, and then feed them to the waves.
            // This needs the effects to be created, but ignore for now.
            // projectiles -> PrototypeProjectiles.
            // Then, IWeapon -> PrototypeWeapon.
            // Then, the Units.
            // The above function that was hardcoded built the weapons, then the units, then fed them into the waves.

            AssertUtil.AssertNotNull(enemySchema.Components?.Projectiles, "Projectiles not found in enemy schema.");
            AssertUtil.AssertNotNull(enemySchema.Components?.VisualEffects, "VisualEffects not found in enemy schema.");
            AssertUtil.AssertNotNull(enemySchema.Entities, "Entities not found in enemy schema.");

            Dictionary<string, PrototypeProjectile> projectiles = InitProjectileStubs(enemySchema.Components);

            // Ok. Now to create the entities. These are of type PrototypeUnit, which is a unit with a weapon and stats

            Dictionary<string, PrototypeUnit> entities = InitPrototypeUnit(enemySchema.Entities, projectiles, enemySchema.Components.VisualEffects);

            return entities;
        }

    }
}