using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Components.Actions;
using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Prototype
{
    internal class WeaponPrototypeBasic : WeaponPrototype, ICloneable
    {
        public IGameplayAction? CastEffect { get; set; }
        public float Cooldown { get; set; }
        public float CastRange { get; set; }

        public WeaponPrototypeBasic(IGameplayAction? castEffect, float cooldown, float castRange)
        {
            CastEffect = castEffect;
            Cooldown = cooldown;
            CastRange = castRange;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
        public WeaponPrototypeBasic SetDamage(float damage)
        {
            IGameplayAction? newEffect;
            switch (CastEffect)
            {
                case ProjectileAction projEffect: // Created a projectile. That projectile deals damage.
                    var oldProj = projEffect.Prototype;
                    var newProj = new ProjectilePrototype(
                        oldProj.Size,
                        oldProj.Speed,
                        oldProj.Lifespan,
                        new DamageAction(damage),
                        oldProj.FillColor);
                    newEffect = new ProjectileAction(newProj);
                    break;
                case DamageAction _: // Already did Damage.
                    newEffect = new DamageAction(damage);
                    break;
                case IGameplayAction _: // Null effect.
                    newEffect = new DamageAction(damage);
                    break;
                default:
                    newEffect = CastEffect; // fallback, or throw
                    break;
            }
            return new WeaponPrototypeBasic(newEffect, Cooldown, CastRange);
        }
    }
}
