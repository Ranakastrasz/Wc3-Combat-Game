using AssertUtils;

using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Projectiles.Prototypes;
using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.Entities.Units.Abilities
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

        public class AbilityBuilder
        {
            AbilityPrototype _prototype;
            public AbilityBuilder(float cooldown, float range)
            {
                _prototype = new AbilityPrototype("","",null, null, cooldown, range);
            }

            public AbilityBuilder WithTargetEffect(IGameplayAction effect)
            {
                _prototype = _prototype with { TargetEffect = effect };
                return this;
            }
            public AbilityBuilder WithCasterEffect(IGameplayAction effect)
            {
                _prototype = _prototype with { CasterEffect = effect };
                return this;
            }
            public AbilityBuilder WithManaCost(float manaCost)
            {
                _prototype = _prototype with { ManaCost = manaCost };
                return this;
            }

            public AbilityPrototype Build() => _prototype;

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
            string id = $"instant_weapon_{damage}_{cooldown}_{range}_{recoilFactor}_{recoilDuration}";
            AbilityPrototype prototype = new AbilityPrototype(id,id,null, null, 1f, 20f).WithDamage(damage);
            
            if (recoilFactor != null && recoilDuration != 0f)
            {
                prototype = prototype with { CasterEffect = CreateRecoilAction(recoilFactor.Value, recoilDuration) };
            }

            return prototype;
        }

        public static AbilityPrototype CreateRangedWeapon(float manaCost, float recoilFactor, float recoilDuration, float speed, float damage, float aoe, float range, float cooldown, float radius, int vertexes, Color color)
        {
            string id = $"ranged_weapon_{manaCost}_{recoilFactor}_{recoilDuration}_{speed}_{damage}_{aoe}_{range}_{cooldown}_{radius}_{vertexes}_{color.ToArgb()}";
            IGameplayAction damageAction = aoe > 0f ? new AoeDamageAction(damage,damage*0.5f,aoe.World()) : new DamageAction(damage);


            ProjectilePrototype weaponProjectile = new ProjectilePrototype(
                radius,
                speed,
                range*1.1f/speed, // Add a bit of slop. 
                damageAction,
                vertexes,
                color);
            ProjectileAction projectile = new ProjectileAction(weaponProjectile);
            IGameplayAction? recoilAction = recoilDuration > 0f ? CreateRecoilAction(recoilFactor, recoilDuration) : null;

            return new AbilityPrototype(id,id,projectile, recoilAction, cooldown, range, manaCost);
        }


        
        //public static AbilityPrototype CreateSnareWeapon()
        //{
        //    ProjectileAction projectile = new ProjectileAction(new ProjectilePrototype("Snare Projectile", 2.5f, /225, /16f,
        //    new BuffAction(IBuffable.BuffType.Slow, 0.5f, 1f), Color.Cyan));
        //    return new AbilityPrototype(projectile, null, 0.5f, 150f, 5f);
        //}
    }
}
