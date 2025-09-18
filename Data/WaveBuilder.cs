using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AssertUtils;

using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.Waves;

namespace Wc3_Combat_Game.Data
{
    // For now, does basically all of it. Will split later or something.
    public static class WaveBuilder
    {
        public static void BuildWaves(List<Wave> _waves)
        {
            var meleeWeaponBase = new AbilityPrototype(null, null, 1f, 20f);
            var weapon5Damage = meleeWeaponBase.WithDamage(5f);
            var weapon10Damage = meleeWeaponBase.WithDamage(10f);
            var weapon25Damage = meleeWeaponBase.WithDamage(25f);
            var weapon200Damage = meleeWeaponBase.WithDamage(200f);

            var rangedWeaponBase = new AbilityPrototype(
                    new ProjectileAction(new ProjectilePrototype("Ranged Weapon",2.5f, 225f, 4f, null, Color.DarkMagenta)), null,
                    0.5f,
                    150f,10f);

            var rangedWeaponSnare = new AbilityPrototype(
                    new ProjectileAction(new ProjectilePrototype("Snare Projectile",2.5f, 225, 16f,
                        new BuffAction(Entities.Components.Interface.IBuffable.BuffType.Slow,1f,0.5f), Color.Cyan)), null,
                    0.5f,
                    150f,5f);


            var unit = new UnitPrototype("Basic",15f, 2f, 4f, 50f, Color.Brown, 6);
            unit = unit.AddAbility(meleeWeaponBase.WithDamage(5f));
            _waves.Add(new Wave(unit, 32));
        
            unit = new UnitPrototype("Blitz",10f, 0.0f, 4f, 75f, Color.DarkGoldenrod, 3);
            unit = unit.AddAbility(meleeWeaponBase.WithDamage(10f));
            _waves.Add(new Wave(unit, 32));
        
            unit = new UnitPrototype("Blaster",30f, 0.0f, 5f, 40f, Color.Orange, 5);
            unit = unit.AddAbility(rangedWeaponBase.WithDamage(10f));
            _waves.Add(new Wave(unit, 16));
        
            unit = new UnitPrototype("Brute",80f, 2f, 10f, 50f, Color.Brown, 6);
            unit = unit.AddAbility(meleeWeaponBase.WithDamage(25f));
            _waves.Add(new Wave(unit, 8));

            unit = new UnitPrototype("Boss",400f, 0f, 15f, 100f, Color.DarkRed, 4);
            unit = unit.AddAbility(meleeWeaponBase.WithDamage(90f));
            unit = unit.AddAbility(rangedWeaponSnare);
            _waves.Add(new Wave(unit, 1));
        }
        
    }
}