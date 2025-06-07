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
        public Effects.Action? CastEffect { get; set; }
        public float Cooldown { get; set; }
        public float CastRange { get; set; }

        public PrototypeWeaponBasic(Effects.Action? castEffect, float cooldown, float castRange)
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
            Effects.Action? newEffect;
            switch (CastEffect)
            {
                case ActionProjectile projEffect: // Created a projectile. That projectile deals damage.
                    var oldProj = projEffect.Prototype;
                    var newProj = new PrototypeProjectile(
                        oldProj.Size,
                        oldProj.Speed,
                        oldProj.Lifespan,
                        new ActionDamage(damage),
                        oldProj.FillColor);
                    newEffect = new ActionProjectile(newProj);
                    break;
                case ActionDamage _: // Already did Damage.
                    newEffect = new ActionDamage(damage);
                    break;
                case Effects.Action _: // Null effect.
                    newEffect = new ActionDamage(damage);
                    break;
                default:
                    newEffect = CastEffect; // fallback, or throw
                    break;
            }
            return new PrototypeWeaponBasic(newEffect, Cooldown, CastRange);
        }
    }
}
