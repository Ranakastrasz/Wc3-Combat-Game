using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Entities.Components.Abilities;

namespace Wc3_Combat_Game.Entities.Components.Prototype.Abilities
{
    public class AbilityPrototype: ICloneable
    {
        public IGameplayAction? TargetEffect { get; private set; }
        public IGameplayAction? CasterEffect { get; private set; }
        public float Cooldown { get; private set; }
        public float CastRange { get; private set; }
        public float ManaCost { get; private set; }

        public AbilityPrototype(IGameplayAction? targetEffect, IGameplayAction? casterEffect, float cooldown, float castRange, float manaCost = 0f)
        {
            TargetEffect = targetEffect;
            CasterEffect = casterEffect;
            Cooldown = cooldown;
            CastRange = castRange;
            ManaCost = manaCost;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public AbilityPrototype WithDamage(float damage)
        {
            IGameplayAction? newEffect;
            switch(TargetEffect) // Very messy but works for now.
            {
                case ProjectileAction projectileAction: // Created a projectile. That projectile deals damage.
                    var oldProjectile = projectileAction.Prototype;
                    var newProjectile = new ProjectilePrototype(
                        oldProjectile.Name,
                        oldProjectile.Radius,
                        oldProjectile.Speed,
                        oldProjectile.Lifespan,
                        new DamageAction(damage),
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
            return new AbilityPrototype(newEffect, CasterEffect, Cooldown, CastRange);
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
            return this.WithDamage(GetDamage() * multiplier);
        }
    }
}
