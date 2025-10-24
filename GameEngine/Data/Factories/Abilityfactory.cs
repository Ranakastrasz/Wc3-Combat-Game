using System.Buffers.Text;

using AssertUtils;

using Wc3_Combat_Game.Entities.Components.Drawable;
using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Actions.Interface;
using Wc3_Combat_Game.GameEngine.Data.Data;
using Wc3_Combat_Game.Util.UnitConversion;

namespace Wc3_Combat_Game.GameEngine.Data.Factories
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


    public static class AbilityFactory
    {

        public class AbilityBuilder
        {
            AbilityData _prototype;
            public AbilityBuilder(float cooldown, float range)
            {
                _prototype = new AbilityData("<id>", "<name>", manaCost: 0f, cooldown: cooldown, castRange: range, targetEffect: null, casterEffect: null);
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

            public AbilityData Build() => _prototype;

        }

        /*
         * Recoil is a debuff that slows an attacker after their attack.
         * Used to simulate the time it takes to recover from a powerful attack.
         */
        public static IGameplayAction CreateRecoilAction(float recoilFactor, float recoilDuration)
        {
            AssertUtil.NotNegative(recoilFactor);
            AssertUtil.Positive(recoilDuration);

            return new BuffAction("", IBuffable.BuffType.Slow, recoilFactor, recoilDuration);
            
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
        public static AbilityData CreateInstantWeapon(float damage, float cooldown, float range, float? recoilFactor = null, float recoilDuration = 0f)
        {
            string id = $"instant_weapon_{damage}_{cooldown}_{(range == float.PositiveInfinity ? "infinite" : range.ToString())}_{recoilFactor}_{recoilDuration}";
            if(DataManager.TryGetAbility(id, out AbilityData? existingPrototype))
            {
                return existingPrototype!;
            }
            AbilityData prototype = new AbilityData(id,id,manaCost: 0f, cooldown: 1f, castRange: 20f, targetEffect: null, casterEffect: null).WithDamage(damage);
            
            if (recoilFactor != null && recoilDuration != 0f)
            {
                prototype = prototype with { CasterEffect = CreateRecoilAction(recoilFactor.Value, recoilDuration) };
            }

            return prototype;
        }

        public static AbilityData CreateRangedWeapon(float manaCost, float cooldown, float damage, float aoe, float range, float speed, float radius, float recoilFactor, float recoilDuration, int polygonCount, Color color)
        {
            string baseId = $"ranged_weapon_{manaCost}_{recoilFactor}_{recoilDuration}_{speed}_{damage}_{aoe}_{(range == float.PositiveInfinity ? "infinite" : range.ToString())}_{cooldown}_{radius}_{polygonCount}_{color.R}-{color.G}-{color.B}-{color.A}";
            if(DataManager.TryGetAbility(baseId, out AbilityData? existingPrototype))
            {
                return existingPrototype!;
            }
            string impactActionId = $"{baseId}_impact_action";

            IGameplayAction damageAction = aoe > 0f
                ? new AoeDamageAction(impactActionId,damage,damage*0.5f,aoe.World())
                : new DamageAction(impactActionId,damage);

            DataManager.RegisterGameplayAction(damageAction);

            string projectileID = $"{baseId}_projectile";
            ProjectileData weaponProjectile = new ProjectileData(
                projectileID,
                radius,
                speed,
                range*1.1f/speed, // Add a bit of slop. 
                damageAction,
                polygonCount,
                color) with { ID = projectileID };
            ProjectileAction projectile = new ProjectileAction("", weaponProjectile);
            IGameplayAction? recoilAction = recoilDuration > 0f ? CreateRecoilAction(recoilFactor, recoilDuration) : null;

            DataManager.RegisterProjectile(weaponProjectile);

            return new AbilityData(baseId, baseId, manaCost, cooldown, range, projectile, recoilAction);
        }

        public static AbilityData WithFan(this AbilityData abilityData, int projectileCount, float fullSpreadAngleDeg)
        {
            ProjectileAction? action;
            if((action = abilityData.TargetEffect as ProjectileAction) != null)
            {
                action = action with { ProjectileCount = projectileCount, FullSpreadAngleDeg = fullSpreadAngleDeg };
                abilityData = abilityData with { TargetEffect = action };
            }
            else
            {
                throw new InvalidOperationException("AbilityData does not have a ProjectileAction as its TargetEffect.");
            }

            return abilityData;
        }

        public static AbilityData WithID(this AbilityData abilityData, string id, string name)
        {
            return abilityData with { ID = id, Name = name };
        }



        //public static AbilityPrototype CreateSnareWeapon()
        //{
        //    ProjectileAction projectile = new ProjectileAction(new ProjectilePrototype("Snare Projectile", 2.5f, /225, /16f,
        //    new BuffAction(IBuffable.BuffType.Slow, 0.5f, 1f), Color.Cyan));
        //    return new AbilityPrototype(projectile, null, 0.5f, 150f, 5f);
        //}
    }
}
