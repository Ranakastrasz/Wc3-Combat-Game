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

            PrototypeManager.RegisterAbility(new AbilityPrototype("ranged_weapon","Light Death Bolt",
                    new ProjectileAction(new ProjectilePrototype(2.5f, 150f, 4f,
                        new DamageAction(10f), int.MaxValue, Color.DarkMagenta)), null,
                    1.5f,
                    150f,10f));
            PrototypeManager.RegisterAbility(new AbilityPrototype("ranged_weapon_light","Basic Arrow",
                    new ProjectileAction(new ProjectilePrototype(1.5f, 225f, 4f,
                        new DamageAction(5f), 2, Color.White)), null,
                    1.5f,
                    150f,10f));

            var heavyProjectile = new ProjectilePrototype(4f, 250f, int.MaxValue,
                        new DamageAction(45f), int.MaxValue, Color.DarkMagenta);

            PrototypeManager.RegisterAbility(new AbilityPrototype("ranged_weapon_heavy","Death Bolt",
                    new ProjectileAction(heavyProjectile), null,
                    2f,
                    225f,10f));

            var spreadHeavyAction = new ProjectileAction(heavyProjectile)
            {
                ProjectileCount = 5,
                FullSpreadAngleDeg = 60f
            };

            PrototypeManager.RegisterAbility(new AbilityPrototype("ranged_weapon_heavy_fan","Death Bolt (Fan)",
                    spreadHeavyAction, null,
                    5f,
                    200f,10f));

            PrototypeManager.RegisterAbility(new AbilityPrototype("ranged_weapon_snare","Shockwave (Snare)",
                    new ProjectileAction(new ProjectilePrototype(2.5f, 225, 16f,
                        new BuffAction(IBuffable.BuffType.Slow,0.5f,1f),int.MaxValue, Color.Goldenrod)), null,
                    0.5f,
                    150f,5f));

            PrototypeManager.RegisterAbility(new AbilityPrototype("enemy_sprint","Sprint",
                null,
                new BuffAction(IBuffable.BuffType.Speed, 6f, 0.25f),
                3f,
                225f));



            PrototypeManager.RegisterUnit(
                new UnitPrototype("basic_enemy", "Basic", 15f, 2f, 4f, 50f, Color.Brown, 6)
                .AddAbility(AbilityFactory.CreateInstantWeapon(5f, 1, 20, 0.25f, 0.5f).ID));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("blitz_enemy", "Blitz", 10f, 0.0f, 4f, 50f, Color.DarkGoldenrod, 3)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(10f, 1, 20, 0.25f, 0.5f))
                .AddAbility("enemy_sprint"));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("blaster_enemy","Blaster",30f, 0.0f, 5f, 40f, Color.Orange, 5)
                .AddAbility("ranged_weapon"));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("brute_enemy","Brute",80f, 2f, 10f, 50f, Color.Brown, 6)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(25f, 1, 20, 0.25f, 0.5f)));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("boss_enemy","Boss",400f, 0f, 15f, 80f, Color.DarkRed, 4)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(90f, 2, 20, 0.25f, 0.5f))
                .AddAbility("enemy_sprint"));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("swarmer_enemy","Swarmer",10f, 0.0f, 3f, 25f, Color.OrangeRed, int.MaxValue)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(5f, 1, 20, 0.25f, 0.5f))
                .AddAbility("enemy_sprint"));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("light_blaster_enemy","Light Blaster",15f, 0.0f, 3.5f, 30f, Color.SaddleBrown, 5)
                .AddAbility("ranged_weapon_light"));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("elite_brute_enemy","Elite Brute",100f, 3f, 10f, 60f, Color.Maroon, 6)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(30f, 1, 20, 0.25f, 0.5f))
                .AddAbility("ranged_weapon_snare"));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("elite_blaster_enemy","Elite Blaster",320f, 0f, 6f, 50f, Color.Purple, 5)
                .AddAbility("ranged_weapon_heavy")
                .AddAbility("ranged_weapon_snare"));

            PrototypeManager.RegisterUnit(
                new UnitPrototype("elite_boss_enemy","Elite Boss",500f, 0f, 15f, 75f, Color.DarkMagenta, 4)
                .AddAbility("ranged_weapon_heavy_fan")
                .AddAbility("ranged_weapon_heavy")
                .AddAbility("ranged_weapon_snare"));
        }
    }
}