using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Actions.Interface;

namespace Wc3_Combat_Game.GameEngine.Data.Data
{
    public record AbilityData
    {
        public string ID;
        public string Name;
        public IGameplayAction? TargetEffect;
        public IGameplayAction? CasterEffect;
        public float Cooldown;
        public float CastRange;
        public float ManaCost;

        public AbilityData(string id, string name, float manaCost, float cooldown, float castRange, IGameplayAction? targetEffect, IGameplayAction? casterEffect)
        {
            ID = id;
            Name = name;
            TargetEffect = targetEffect;
            CasterEffect = casterEffect;
            Cooldown = cooldown;
            CastRange = castRange;
            ManaCost = manaCost;
        }

        public AbilityData WithDamage(float damage)
        {
            IGameplayAction? newEffect;
            switch(TargetEffect) // Very messy but works for now.
            {
                case ProjectileAction projectileAction: // Created a projectile. That projectile deals damage.
                    var oldProjectile = projectileAction.Prototype;
                    var newProjectile = new ProjectileData(ID,
                        oldProjectile.Radius,
                        oldProjectile.Speed,
                        oldProjectile.Lifespan,
                        new DamageAction("",damage),
                        int.MaxValue,
                        oldProjectile.Color);
                    newEffect = new ProjectileAction("", newProjectile);
                    break;
                case DamageAction : // Already did Damage.
                    newEffect = new DamageAction("", damage);
                    break;
                case null : // Null effect.
                    newEffect = new DamageAction("", damage);
                    break;
                default:
                    newEffect = TargetEffect; // fallback, or throw
                    break;
            }
            return new AbilityData(id: ID, name: Name, manaCost: ManaCost, cooldown: Cooldown, castRange: CastRange, targetEffect: newEffect, casterEffect: CasterEffect);
        }

        internal float GetDamage()
        {
            float damageValue = 0f;
            switch(TargetEffect) // Very messy but works for now.
            {
                case ProjectileAction projectileAction:
                    if(projectileAction.Prototype.ImpactActions[0] is DamageAction impactDamageAction)
                    {
                        damageValue = impactDamageAction.Damage;
                    }
                    break;
                case DamageAction directDamageAction: // Already did Damage.
                    damageValue = directDamageAction.Damage;
                    break;
                case null: // Null effect.
                    break;
                default:
                    break;
            }
            return damageValue;
        }

        internal AbilityData WithIncreasedDamage(float multiplier)
        {
            return WithDamage(GetDamage() * multiplier);
        }
    }
}
