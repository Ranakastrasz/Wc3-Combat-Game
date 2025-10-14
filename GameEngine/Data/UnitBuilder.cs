using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Entities.Projectiles.Prototypes;
using Wc3_Combat_Game.Entities.Units.Abilities;
using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.Entities.Units.Prototypes;
using Wc3_Combat_Game.GameEngine.Actions;

namespace Wc3_Combat_Game.GameEngine.Data
{
    public class UnitBuilder
    {

        public static void BuildUnits()
        {

            var rangedWeapon = new AbilityPrototype("ranged_weapon","Light Death Bolt",
                    new ProjectileAction(new ProjectilePrototype(2.5f, 150f, 4f,
                        new DamageAction(10f), int.MaxValue, Color.DarkMagenta)), null,
                    1.5f,
                    150f,10f);
            var rangedWeaponLight = new AbilityPrototype("light_ranged_weapon","Basic Arrow",
                    new ProjectileAction(new ProjectilePrototype(1.5f, 225f, 4f,
                        new DamageAction(5f), 2, Color.White)), null,
                    1.5f,
                    150f,10f);

            var heavyProjectile = new ProjectilePrototype(4f, 250f, int.MaxValue,
                        new DamageAction(45f), int.MaxValue, Color.DarkMagenta);

            var rangedWeaponHeavy = new AbilityPrototype("heavy_ranged_weapon","Death Bolt",
                    new ProjectileAction(heavyProjectile), null,
                    2f,
                    225f,10f);

            var spreadHeavyAction = new ProjectileAction(heavyProjectile)
            {
                ProjectileCount = 5,
                FullSpreadAngleDeg = 60f
            };

            var rangedWeaponHeavyFan = new AbilityPrototype("heavy_ranged_weapon_fan","Death Bolt (Fan)",
                    spreadHeavyAction, null,
                    5f,
                    200f,10f);

            var rangedWeaponSnare = new AbilityPrototype("snare_ranged_weapon","Shockwave (Snare)",
                    new ProjectileAction(new ProjectilePrototype(2.5f, 225, 16f,
                        new BuffAction(IBuffable.BuffType.Slow,0.5f,1f),int.MaxValue, Color.Cyan)), null,
                    0.5f,
                    150f,5f);



            UnitPrototype unit;
            PrototypeManager.RegisterUnit(new UnitPrototype("basic_enemy", "Basic", 15f, 2f, 4f, 50f, Color.Brown, 6)
            .AddAbility(AbilityFactory.CreateInstantWeapon(5f, 1, 20, 0.25f, 0.5f)));


            unit = new UnitPrototype("blitz_enemy","Blitz",10f, 0.0f, 4f, 75f, Color.DarkGoldenrod, 3);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(10f, 1, 20, 0.25f, 0.5f));
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("blaster_enemy","Blaster",30f, 0.0f, 5f, 40f, Color.Orange, 5);
            unit = unit.AddAbility(rangedWeapon);
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("brute_enemy","Brute",80f, 2f, 10f, 50f, Color.Brown, 6);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(25f, 1, 20, 0.25f, 0.5f));
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("boss_enemy","Boss",400f, 0f, 15f, 100f, Color.DarkRed, 4);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(90f, 2, 20, 0.25f, 0.5f));
            unit = unit.AddAbility(rangedWeaponSnare);
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("swarmer_enemy","Swarmer",10f, 0.0f, 3f, 40f, Color.OrangeRed, int.MaxValue);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(5f, 1, 20, 0.25f, 0.5f));
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("light_blaster_enemy","Light Blaster",15f, 0.0f, 3.5f, 30f, Color.SaddleBrown, 5);
            unit = unit.AddAbility(rangedWeaponLight);
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("elite_brute_enemy","Elite Brute",100f, 3f, 10f, 60f, Color.Maroon, 6);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(30f, 1, 20, 0.25f, 0.5f));
            unit = unit.AddAbility(rangedWeaponSnare);
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("elite_blaster_enemy","Elite Blaster",320f, 0f, 6f, 50f, Color.Purple, 5);
            unit = unit.AddAbility(rangedWeaponHeavy);
            unit = unit.AddAbility(rangedWeaponSnare);
            PrototypeManager.RegisterUnit(unit);

            unit = new UnitPrototype("elite_boss_enemy","Elite Boss",500f, 0f, 15f, 75f, Color.DarkMagenta, 4);
            unit = unit.AddAbility(rangedWeaponHeavyFan);
            unit = unit.AddAbility(rangedWeaponHeavy);
            unit = unit.AddAbility(rangedWeaponSnare);
            PrototypeManager.RegisterUnit(unit);
        }
    }
}