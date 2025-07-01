using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Components.Actions;
using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Prototype.Weapons
{
    internal class WeaponPrototypeBasic : WeaponPrototype, ICloneable
    {
        public IGameplayAction? CastEffect { get; private set; }
        public float Cooldown { get; private set; }
        public float CastRange { get; private set; }
        public float ManaCost { get; private set; }

        public WeaponPrototypeBasic(IGameplayAction? castEffect, float cooldown, float castRange, float manaCost = 0f)
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
        public WeaponPrototypeBasic SetDamage(float damage)
        {
            IGameplayAction? newEffect;
            switch (CastEffect) // Very messy but works for now.
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
