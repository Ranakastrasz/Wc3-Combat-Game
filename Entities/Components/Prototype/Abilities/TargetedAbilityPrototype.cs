using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Actions.Interface;

namespace Wc3_Combat_Game.Entities.Components.Prototype.Abilities
{
    internal class TargetedAbilityPrototype: AbilityPrototype, ICloneable
    {
        public IGameplayAction? CastEffect { get; private set; }
        public float Cooldown { get; private set; }
        public float CastRange { get; private set; }
        public float ManaCost { get; private set; }

        public TargetedAbilityPrototype(IGameplayAction? castEffect, float cooldown, float castRange, float manaCost = 0f)
        {
            CastEffect = castEffect;
            Cooldown = cooldown;
            CastRange = castRange;
            ManaCost = manaCost;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
        public TargetedAbilityPrototype WithDamage(float damage)
        {
            IGameplayAction? newEffect;
            switch(CastEffect) // Very messy but works for now.
            {
                case ProjectileAction projectileAction: // Created a projectile. That projectile deals damage.
                    var oldProjectile = projectileAction.Prototype;
                    var newProjectile = new ProjectilePrototype(
                        oldProjectile.Radius,
                        oldProjectile.Speed,
                        oldProjectile.Lifespan,
                        new DamageAction(damage),
                        oldProjectile.FillColor);
                    newEffect = new ProjectileAction(newProjectile);
                    break;
                case DamageAction : // Already did Damage.
                    newEffect = new DamageAction(damage);
                    break;
                case null : // Null effect.
                    newEffect = new DamageAction(damage);
                    break;
                default:
                    newEffect = CastEffect; // fallback, or throw
                    break;
            }
            return new TargetedAbilityPrototype(newEffect, Cooldown, CastRange);
        }

        internal float GetDamage()
        {
            float damageValue = 0f;
            switch(CastEffect) // Very messy but works for now.
            {
                case ProjectileAction projectileAction:
                    if(projectileAction.Prototype.ImpactEffect is DamageAction impactDamageAction)
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
    }
}
