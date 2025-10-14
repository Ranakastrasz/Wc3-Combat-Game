using Wc3_Combat_Game.Entities.Projectiles.Prototypes;
using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Actions.Interface;

namespace Wc3_Combat_Game.Entities.Units.Abilities
{
    public record AbilityPrototype
    {
        public string ID;
        public string Name;
        public IGameplayAction? TargetEffect;
        public IGameplayAction? CasterEffect;
        public float Cooldown;
        public float CastRange;
        public float ManaCost;

        public AbilityPrototype(string id, string name, IGameplayAction? targetEffect, IGameplayAction? casterEffect, float cooldown, float castRange, float manaCost = 0f)
        {
            ID = id;
            Name = name;
            TargetEffect = targetEffect;
            CasterEffect = casterEffect;
            Cooldown = cooldown;
            CastRange = castRange;
            ManaCost = manaCost;
        }

        public AbilityPrototype WithDamage(float damage)
        {
            IGameplayAction? newEffect;
            switch(TargetEffect) // Very messy but works for now.
            {
                case ProjectileAction projectileAction: // Created a projectile. That projectile deals damage.
                    var oldProjectile = projectileAction.Prototype;
                    var newProjectile = new ProjectilePrototype(
                        oldProjectile.Radius,
                        oldProjectile.Speed,
                        oldProjectile.Lifespan,
                        new DamageAction(damage),
                        int.MaxValue,
                        oldProjectile.Color);
                    newEffect = new ProjectileAction(newProjectile);
                    break;
                case DamageAction : // Already did Damage.
                    newEffect = new DamageAction(damage);
                    break;
                case null : // Null effect.
                    newEffect = new DamageAction(damage);
                    break;
                default:
                    newEffect = TargetEffect; // fallback, or throw
                    break;
            }
            return new AbilityPrototype(ID,Name, newEffect, CasterEffect, Cooldown, CastRange);
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

        internal AbilityPrototype WithIncreasedDamage(float multiplier)
        {
            return WithDamage(GetDamage() * multiplier);
        }
    }
}
