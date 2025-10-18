using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Data.Data;
using Wc3_Combat_Game.GameEngine.Data.Factories;

namespace Wc3_Combat_Game.GameEngine.Data
{
    public class UnitBuilder
    {

        public static void BuildUnits()
        {

            DataManager.RegisterAbility(new AbilityData(
                id: "ranged_weapon", name: "Light Death Bolt",
                    manaCost: 10f, cooldown: 1.5f,
                    castRange: 150f,
                    targetEffect: new ProjectileAction("", new ProjectileData(
                        radius: 2.5f, speed: 150f, lifespan: 4f,
                        impactAction: new DamageAction("", 10f),
                        polygonCount: int.MaxValue, color: Color.DarkMagenta)),
                    casterEffect: null));

            DataManager.RegisterAbility(new AbilityData(
                id: "ranged_weapon_light", name: "Basic Arrow",
                manaCost: 0f, cooldown: 1.5f,
                castRange: 150f,
                targetEffect: new ProjectileAction("", new ProjectileData(
                    radius: 1.5f, speed: 225f, lifespan: 4f,
                    impactAction: new DamageAction("", 5f),
                    polygonCount: 2, color: Color.White)),
                casterEffect: null));

            var heavyProjectile = new ProjectileData(
                radius: 4f, speed: 250f, lifespan: int.MaxValue,
                impactAction: new DamageAction("",45f), polygonCount: int.MaxValue, color: Color.DarkMagenta);

            var spreadHeavyAction = new ProjectileAction("",heavyProjectile)
            {
                ProjectileCount = 5,
                FullSpreadAngleDeg = 60f
            };

            DataManager.RegisterAbility(new AbilityData(
                id: "ranged_weapon_heavy", name: "Death Bolt",
                manaCost: 0f, cooldown: 2f,
                castRange: 225f,
                targetEffect: new ProjectileAction("", heavyProjectile), casterEffect: null));


            DataManager.RegisterAbility(new AbilityData(
                id: "ranged_weapon_heavy_fan", name: "Death Bolt (Fan)",
                manaCost: 0f, cooldown: 5f,
                castRange: 200f,
                targetEffect: spreadHeavyAction,
                casterEffect: null));

            DataManager.RegisterAbility(new AbilityData(
                id: "ranged_weapon_snare", name: "Shockwave (Snare)",
                manaCost: 0f, cooldown: 0.5f,
                castRange: 150f,
                targetEffect: new ProjectileAction("", new ProjectileData(
                    radius: 2.5f, speed: 225f, lifespan: 16f,
                    impactAction: new BuffAction("", IBuffable.BuffType.Slow, 0.5f, 1f),
                    polygonCount: int.MaxValue, color: Color.Goldenrod)),
                casterEffect: null));

            DataManager.RegisterAbility(new AbilityData(
                id: "enemy_sprint", name: "Sprint",
                manaCost: 0f,
                cooldown: 3f,
                castRange: 225f,
                targetEffect: null,
                casterEffect: new BuffAction("",
                    type: IBuffable.BuffType.Speed, factor: 6f, duration: 0.25f)));



            DataManager.RegisterUnit(new UnitData(
                id: "basic_enemy", name: "Basic",
                MaxLife: 15f, LifeRegen: 2f,
                Radius: 4f, Speed: 50f,
                Color: Color.Brown, PolygonCount: 6)
                .AddAbility(AbilityFactory.CreateInstantWeapon(damage: 5f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f).ID));

            DataManager.RegisterUnit(new UnitData(
                id: "blitz_enemy", name: "Blitz",
                MaxLife: 10f, LifeRegen: 0.0f,
                Radius: 4f, Speed: 50f,
                Color: Color.DarkGoldenrod, PolygonCount: 3)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 10f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("enemy_sprint"));

            DataManager.RegisterUnit(
                new UnitData(id: "blaster_enemy", name: "Blaster",
                MaxLife: 30f, LifeRegen: 0.0f,
                Radius: 5f, Speed: 40f,
                Color: Color.Orange, PolygonCount: 5)
                .AddAbility("ranged_weapon"));

            DataManager.RegisterUnit(
                new UnitData(id: "brute_enemy", name: "Brute",
                MaxLife: 80f, LifeRegen: 2f,
                Radius: 10f, Speed: 50f,
                Color: Color.Brown, PolygonCount: 6)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 25f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f)));

            DataManager.RegisterUnit(
                new UnitData(id: "boss_enemy", name: "Boss",
                MaxLife: 400f, LifeRegen: 0f,
                Radius: 15f, Speed: 80f,
                Color: Color.DarkRed, PolygonCount: 4)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 90f, cooldown: 2f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("enemy_sprint"));

            DataManager.RegisterUnit(
                new UnitData(id: "swarmer_enemy", name: "Swarmer",
                MaxLife: 10f, LifeRegen: 0.0f,
                Radius: 3f, Speed: 25f,
                Color: Color.OrangeRed, PolygonCount: int.MaxValue)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 5f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("enemy_sprint"));

            DataManager.RegisterUnit(
                new UnitData(id: "light_blaster_enemy", name: "Light Blaster",
                MaxLife: 15f, LifeRegen: 0.0f,
                Radius: 3.5f, Speed: 30f,
                Color: Color.SaddleBrown, PolygonCount: 5)
                .AddAbility("ranged_weapon_light"));

            DataManager.RegisterUnit(
                new UnitData(id: "elite_brute_enemy", name: "Elite Brute",
                MaxLife: 100f, LifeRegen: 3f,
                Radius: 10f, Speed: 60f,
                Color: Color.Maroon, PolygonCount: 6)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 30f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("ranged_weapon_snare"));

            DataManager.RegisterUnit(
                new UnitData(id: "elite_blaster_enemy", name: "Elite Blaster",
                MaxLife: 320f, LifeRegen: 0f,
                Radius: 6f, Speed: 50f,
                Color: Color.Purple, PolygonCount: 5)
                .AddAbility("ranged_weapon_heavy")
                .AddAbility("ranged_weapon_snare"));

            DataManager.RegisterUnit(
                new UnitData(id: "elite_boss_enemy", name: "Elite Boss",
                MaxLife: 500f, LifeRegen: 0f,
                Radius: 15f, Speed: 75f,
                Color: Color.DarkMagenta, PolygonCount: 4)
                .AddAbility("ranged_weapon_heavy_fan")
                .AddAbility("ranged_weapon_heavy")
                .AddAbility("ranged_weapon_snare"));
        }
    }
}