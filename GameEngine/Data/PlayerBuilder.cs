using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Data.Data;
using Wc3_Combat_Game.GameEngine.Data.Factories;

namespace Wc3_Combat_Game.GameEngine.Data
{
    public static class PlayerBuilder
    {

        public static void BuildPlayer()
        {

            DataManager.RegisterAbility(
                AbilityFactory.CreateRangedWeapon(
                    manaCost: 3f, cooldown: 0.2f,
                    damage: 10f, aoe: 0f,
                    range: int.MaxValue, speed: 600f, radius: 2f,
                    recoilFactor: 0.5f, recoilDuration: 0.5f,
                    polygonCount: 3, color: Color.Orange)
                with { ID = "manabolt", Name = "Mana bolt" }
            );

            DataManager.RegisterAbility(
            AbilityFactory.CreateRangedWeapon(
                manaCost: 15f, cooldown: 1f,
                damage: 30f, aoe: 32f,
                range: int.MaxValue, speed: 450f, radius: 3f,
                recoilFactor: 0.5f, recoilDuration: 1f,
                polygonCount: int.MaxValue, color: Color.Orange)
                with { ID = "manabomb", Name = "Mana bomb" }
            );

            DataManager.RegisterAbility(
                new AbilityData(
                    id: "sprint", name: "Sprint",
                manaCost: 15f,
                cooldown: 3f,
                castRange: 0f,
                targetEffect: null,
                casterEffect: new BuffAction("", IBuffable.BuffType.Speed, factor: 3f, duration: 0.25f))
            );

            DataManager.RegisterUnit(
                new UnitData(id: "player", name: "Player",
                MaxLife: 100f, LifeRegen: 3f,
                Radius: 5f, Speed: 150f,
                Color: Color.Green, PolygonCount: 0)
                .AddAbility("manabolt")
                .AddAbility("manabomb")
                .AddAbility("sprint")
                with { MaxMana = 200f, ManaRegen = 6f }
            );


        }

        public static void RegisterHotkeys()
        {
            // Register hotkeys for player abilities
            // Manabolt -> Left mouse click.
            // Manabomb, -> Q
            // Sprint, -> Spacebar.

            // Originally, before deciding on WASD, it was going to be
            /*
             F - Manabolt
             D - Arcane Curse
             S - Sprint
             A - Arcane Elemental
             R - Manabomb
             E - Shield
             W - Blink
             */
            // However, A new method is going to be needed.
            // 
            /*
             Movement Utility   Spacebar    Sprint      Excellent choice for a phase/dodge/sprint key in a top-down game.
            Core Attack	        LMB	        Manabolt    The primary attack/spammable skill.
            Flask/Restore       1           Flask       Dedicates the top-row index finger key to the essential panic/sustain button.
            Secondary Attack    Q           Manabomb            Your current, well-placed tactical attack.
            Defensive/Utility   E           Shield              Easy to hit with the middle finger for a quick defense.
            Mobility/Dodge      F           Blink               Reserve the highly accessible 'F' for an instant, crucial action.
            Tactical/CC	        R           Arcane Curse        A tactical, area-of-effect, or cooldown-based spell.
            Summon/Ultimate	    2           Arcane Elemental    Placed on a number key as it's often a "set-and-forget" or long-cooldown skill.
             */
        }
    }
}