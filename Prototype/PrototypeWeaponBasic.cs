using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Prototype
{
    internal class PrototypeWeaponBasic : PrototypeWeapon, ICloneable
    {
        public Effect? CastEffect { get; set; }
        public float Cooldown { get; set; }
        public float CastRange { get; set; }

        public PrototypeWeaponBasic(Effect? castEffect, float cooldown, float castRange)
        {
            CastEffect = castEffect;
            Cooldown = cooldown;
            CastRange = castRange;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public PrototypeWeaponBasic SetDamage(float damage)
        {
            Effect? newEffect;
            switch (CastEffect)
            {
                case EffectProjectile projEffect: // Created a projectile. That projectile deals damage.
                    var oldProj = projEffect.Prototype;
                    var newProj = new PrototypeProjectile(
                        oldProj.Size,
                        oldProj.Speed,
                        oldProj.Lifespan,
                        new EffectDamage(damage),
                        oldProj.FillColor);
                    newEffect = new EffectProjectile(newProj);
                    break;
                case EffectDamage _: // Already did Damage.
                    newEffect = new EffectDamage(damage);
                    break;
                case Effect _: // Null effect.
                    newEffect = new EffectDamage(damage);
                    break;
                default:
                    newEffect = CastEffect; // fallback, or throw
                    break;
            }
            return new PrototypeWeaponBasic(newEffect, Cooldown, CastRange);
        }
    }
}
