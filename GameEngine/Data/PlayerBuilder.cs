using Wc3_Combat_Game.Entities.Units.Abilities;
using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.Entities.Units.Prototypes;
using Wc3_Combat_Game.GameEngine.Actions;

namespace Wc3_Combat_Game.GameEngine.Data
{
    public static class PlayerBuilder
    {

        private static AbilityPrototype BuildManabolt()
        {
            AbilityPrototype weapon = AbilityFactory.CreateRangedWeapon(3f,0.5f,0.5f,600f,10f,0f,int.MaxValue,0.2f,2.5f,3,Color.Orange);
            weapon = weapon with { ID = "manabolt", Name = "Mana bolt" };
            return weapon;
        }

        private static AbilityPrototype BuildManabomb()
        {
            AbilityPrototype weapon = AbilityFactory.CreateRangedWeapon(20f,0.5f,1f,450f,30f,32f,int.MaxValue,1f,5f,int.MaxValue,Color.Orange);
            weapon = weapon with { ID = "manabomb", Name = "Mana bomb" };
            return weapon;
        }

        private static AbilityPrototype BuildSprint()
        { 
            AbilityPrototype sprint = new AbilityPrototype("sprint","Sprint",
                null,
                new BuffAction(IBuffable.BuffType.Speed, 3f, 0.25f),
                3f,
                0f,
                15f);
            return sprint;
        }

        public static void BuildPlayer()
        {

            AbilityPrototype manabolt = BuildManabolt();
            AbilityPrototype manabomb = BuildManabomb();
            AbilityPrototype sprint = BuildSprint();

            PrototypeManager.RegisterAbility(manabolt);
            PrototypeManager.RegisterAbility(manabomb);
            PrototypeManager.RegisterAbility(sprint);


            // Temperary unsafe crap til I have proper builder.

            //ProjectileAction targetEffect = weapon.TargetEffect as ProjectileAction;
            //targetEffect = targetEffect with { ProjectileCount = 3, FullSpreadAngleDeg = 15f };
            //weapon = weapon with { TargetEffect = targetEffect };

            UnitPrototype playerUnit = new("player","Player", 100f,  3f, 5f, 150f, Color.Green, 0);
            playerUnit = playerUnit.AddAbility("manabolt");
            playerUnit = playerUnit.AddAbility("manabomb");
            playerUnit = playerUnit.AddAbility("sprint");

            playerUnit = playerUnit with { MaxMana = 100, ManaRegen = 3f };

            PrototypeManager.RegisterUnit(playerUnit);


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