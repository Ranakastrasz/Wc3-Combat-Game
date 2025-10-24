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
            DataManager.RegisterAbility(AbilityFactory.CreateRangedWeapon(
                manaCost: 10f, cooldown: 1.5f,
                damage: 10f, aoe: 0f,
                range: 300f, speed: 150f, radius: 2.5f,
                recoilFactor: 0f, recoilDuration: 0f,
                polygonCount: int.MaxValue, color: Color.DarkMagenta)
                with { ID = "ranged_weapon", Name = "Light Death Bolt" });

            DataManager.RegisterAbility(AbilityFactory.CreateRangedWeapon(
                manaCost: 0f, cooldown:1.5f,
                damage:5f, aoe:0f,
                range:150f, speed:225f, radius:1.5f,
                recoilFactor:0f, recoilDuration:0f,
                polygonCount:2, color:Color.White)
                with { ID = "ranged_weapon_light", Name = "Basic Arrow" });

            DataManager.RegisterAbility(AbilityFactory.CreateRangedWeapon(
                manaCost: 0f, cooldown: 2f,
                damage: 45f, aoe: 0f,
                range: 225f, speed: 250f, radius: 4f,
                recoilFactor: 0f, recoilDuration: 0f,
                polygonCount: int.MaxValue, color: Color.Purple)
                with
            { ID = "ranged_weapon_heavy", Name = "Death Coil" });

            DataManager.RegisterAbility(AbilityFactory.CreateRangedWeapon(
                manaCost: 0f, cooldown: 5f,
                damage: 45f, aoe: 0f,
                range: 225f, speed: 250f, radius: 4f,
                recoilFactor: 0f, recoilDuration: 0f,
                polygonCount: int.MaxValue, color: Color.Purple)
                .WithFan(5, 60f)
                .WithID("ranged_weapon_heavy_fan", "Death Coil Fan")
                with
            { CastRange = 200 });

            DataManager.RegisterAbility(new AbilityData(
                id: "ranged_weapon_snare", name: "Shockwave (Snare)",
                manaCost: 0f, cooldown: 0.5f,
                castRange: 150f,
                targetEffect: new ProjectileAction("", new ProjectileData(
                    id: "snare_projectile",
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
                ID: "basic_enemy", Name: "Basic",
                MaxLife: 15f, LifeRegen: 2f,
                Radius: 4f, Speed: 50f,
                Color: Color.Brown, PolygonCount: 6)
                .AddAbility(AbilityFactory.CreateInstantWeapon(damage: 5f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f).ID));

            DataManager.RegisterUnit(new UnitData(
                ID: "blitz_enemy", Name: "Blitz",
                MaxLife: 10f, LifeRegen: 0.0f,
                Radius: 4f, Speed: 50f,
                Color: Color.DarkGoldenrod, PolygonCount: 3)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 10f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("enemy_sprint"));

            DataManager.RegisterUnit(
                new UnitData(ID: "blaster_enemy", Name: "Blaster",
                MaxLife: 30f, LifeRegen: 0.0f,
                Radius: 5f, Speed: 40f,
                Color: Color.Orange, PolygonCount: 5)
                .AddAbility("ranged_weapon"));

            DataManager.RegisterUnit(
                new UnitData(ID: "brute_enemy", Name: "Brute",
                MaxLife: 80f, LifeRegen: 2f,
                Radius: 10f, Speed: 50f,
                Color: Color.Brown, PolygonCount: 6)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 25f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f)));

            DataManager.RegisterUnit(
                new UnitData(ID: "boss_enemy", Name: "Boss",
                MaxLife: 400f, LifeRegen: 0f,
                Radius: 15f, Speed: 80f,
                Color: Color.DarkRed, PolygonCount: 4)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 90f, cooldown: 2f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("enemy_sprint"));

            DataManager.RegisterUnit(
                new UnitData(ID: "swarmer_enemy", Name: "Swarmer",
                MaxLife: 10f, LifeRegen: 0.0f,
                Radius: 3f, Speed: 25f,
                Color: Color.OrangeRed, PolygonCount: int.MaxValue)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 5f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("enemy_sprint"));

            DataManager.RegisterUnit(
                new UnitData(ID: "light_blaster_enemy", Name: "Light Blaster",
                MaxLife: 15f, LifeRegen: 0.0f,
                Radius: 3.5f, Speed: 30f,
                Color: Color.SaddleBrown, PolygonCount: 5)
                .AddAbility("ranged_weapon_light"));

            DataManager.RegisterUnit(
                new UnitData(ID: "elite_brute_enemy", Name: "Elite Brute",
                MaxLife: 100f, LifeRegen: 3f,
                Radius: 10f, Speed: 60f,
                Color: Color.Maroon, PolygonCount: 6)
                .AddWeaponAndRegister(AbilityFactory.CreateInstantWeapon(damage: 30f, cooldown: 1f, range: 20, recoilFactor: 0.25f, recoilDuration: 0.5f))
                .AddAbility("ranged_weapon_snare"));

            DataManager.RegisterUnit(
                new UnitData(ID: "elite_blaster_enemy", Name: "Elite Blaster",
                MaxLife: 320f, LifeRegen: 0f,
                Radius: 6f, Speed: 50f,
                Color: Color.Purple, PolygonCount: 5)
                .AddAbility("ranged_weapon_heavy")
                .AddAbility("ranged_weapon_snare"));

            DataManager.RegisterUnit(
                new UnitData(ID: "elite_boss_enemy", Name: "Elite Boss",
                MaxLife: 500f, LifeRegen: 0f,
                Radius: 15f, Speed: 75f,
                Color: Color.DarkMagenta, PolygonCount: 4)
                .AddAbility("ranged_weapon_heavy_fan")
                .AddAbility("ranged_weapon_heavy")
                .AddAbility("ranged_weapon_snare"));
        }
    }
}