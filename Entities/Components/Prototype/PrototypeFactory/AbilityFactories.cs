using AssertUtils;

using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;

namespace Wc3_Combat_Game.Entities.Components.Prototype.PrototypeFactory
{
    // Factory for weapon varients, I think.
    //
    // Melee attack
    // Ranged attack
    // Ranged Mana costing attack.
    // Ranged hitscan debuff
    // Self buff
    // Self projectile charge thinggy.
    // Summon
    //
    // Really though, probably just make for each ability archetype i end up with.
    // So, include Fan, Extra payload attachment, AOE, etc.
    // Or, its, create ability, attach projectile, attach effect to projectile.


    public class AbilityFactory
    {
        public static readonly Dictionary<string, AbilityPrototype> RegisteredAbilities = new Dictionary<string, AbilityPrototype>();
        public static void RegisterAbility(AbilityPrototype prototype, string id)
        {
            AssertUtil.Assert(!RegisteredAbilities.ContainsKey(id), $"Ability with id {id} is already registered.");
            RegisteredAbilities[id] = prototype;
        }

        /*
         * Recoil is a debuff that slows an attacker after their attack.
         * Used to simulate the time it takes to recover from a powerful attack.
         */
        public static IGameplayAction CreateRecoilAction(float recoilFactor, float recoilDuration)
        {
            AssertUtil.NotNegative(recoilFactor);
            AssertUtil.Positive(recoilDuration);

            return new BuffAction(IBuffable.BuffType.Slow, recoilFactor, recoilDuration);
            
        }

        /*
         * Instant weapon is a weapon that does not use a projectile.
         * It is used for melee attacks, or hitscan attacks.
         * 
         * Damage: The amount of damage the weapon does.
         * Cooldown: The time between attacks.
         * Range: The range of the weapon.
         * RecoilFactor: The factor by which the caster is slowed after attacking. (e.g. 0.5 means the caster is slowed to 50% speed)
         * RecoilDuration: The duration of the recoil effect.
         */
        public static AbilityPrototype CreateInstantWeapon(float damage, float cooldown, float range, float? recoilFactor = null, float recoilDuration = 0f)
        {

            AbilityPrototype prototype = new AbilityPrototype(null, null, 1f, 20f).WithDamage(damage);
            
            if (recoilFactor != null && recoilDuration != 0f)
            {
                prototype = prototype with { CasterEffect = CreateRecoilAction(recoilFactor.Value, recoilDuration) };
            }

            return prototype;
        }
        /*
         
            var weaponProjectile = new ProjectilePrototype(
                "Gun",
                2.5f,
                600f,
                2f,
                new DamageAction(10f),
                Color.Orange);
            return new AbilityPrototype(
                new ProjectileAction(weaponProjectile),
                new BuffAction(Entities.Components.Interface.IBuffable.BuffType.Slow, 0f, 0.5f),
                0.20f,
                float.PositiveInfinity,
                3f);
         */
        /*
         * Inspiration
function AddClassAttack takes integer UnitType, integer Bind, integer SalvoSize, real SalvoReloadTime, real ManaCosts, real Speed, real SpeedMinFactor, real Acceleration, real Friction, real ShadowSize, real AimingOrCruiseZ, real AimingTimeout, real Interception, real LiftFactor, real Randomness, real AngleZ, real TurretAngle, real TurretAngleRange, real Damage, real AOE, real Range, real Cooldown, string Targets, real XOffset, real YOffset, real ZOffset, integer Special, real Scale, integer BigExplosion, string Effect, string Effect2 returns integer
         */
        public static AbilityPrototype CreateRangedWeapon(float manaCost, float recoilFactor, float recoilDuration, float speed, float damage, float aoe, float range, float cooldown, float radius, int vertexes, Color color)
        {
            DamageAction damageAction = /*(aoe > 0f) ? new AoeAction(new DamageAction(Damage), aoe) :*/ new DamageAction(damage);

            ProjectilePrototype weaponProjectile = new ProjectilePrototype(
                radius,
                speed,
                range*1.1f/speed, // Add a bit of slop. 
                damageAction,
                vertexes,
                color);
            ProjectileAction projectile = new ProjectileAction(weaponProjectile);
            IGameplayAction? recoilAction = (recoilDuration > 0f) ? CreateRecoilAction(recoilFactor, recoilDuration) : null;

            return new AbilityPrototype(projectile, recoilAction, cooldown, range, manaCost);
        }
        
        //public static AbilityPrototype CreateSnareWeapon()
        //{
        //    ProjectileAction projectile = new ProjectileAction(new ProjectilePrototype("Snare Projectile", 2.5f, /225, /16f,
        //    new BuffAction(IBuffable.BuffType.Slow, 0.5f, 1f), Color.Cyan));
        //    return new AbilityPrototype(projectile, null, 0.5f, 150f, 5f);
        //}
    }
}
