using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Abilities;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;

namespace Wc3_Combat_Game.Entities.Components.Prototype
{
    public static class UnitFactory
    {

        public static UnitPrototype CreateBasicUnit()
        {
            var unit = new UnitPrototype("Basic", 15f, 2f, 4f, 50f, Color.Brown, 6);
            return unit.AddAbility(AbilityFactory.CreateMeleeWeapon(5f));
        }

        public static UnitPrototype CreateBlitzUnit()
        {
            var unit = new UnitPrototype("Blitz", 10f, 0.0f, 4f, 75f, Color.DarkGoldenrod, 3);
            return unit.AddAbility(AbilityFactory.CreateMeleeWeapon(10f));
        }

        public static UnitPrototype CreateBlasterUnit()
        {
            var unit = new UnitPrototype("Blaster", 30f, 0.0f, 5f, 40f, Color.Orange, 5);
            return unit.AddAbility(AbilityFactory.CreateRangedWeapon(10f));
        }

        public static UnitPrototype CreateBruteUnit()
        {
            var unit = new UnitPrototype("Brute", 80f, 2f, 10f, 50f, Color.Brown, 6);
            return unit.AddAbility(AbilityFactory.CreateMeleeWeapon(25f));
        }

        public static UnitPrototype CreateBossUnit()
        {
            var unit = new UnitPrototype("Boss", 400f, 0f, 15f, 100f, Color.DarkRed, 4);
            unit = unit.AddAbility(AbilityFactory.CreateMeleeWeapon(90f));
            return unit.AddAbility(AbilityFactory.CreateSnareWeapon());
        }
        public static UnitPrototype CreateEliteUnit(UnitPrototype baseUnit)
        {
            var elitePrototype = baseUnit with
            {
                MaxLife = baseUnit.MaxLife * 4,
                LifeRegen = baseUnit.LifeRegen * 4,
                Speed = baseUnit.Speed * 1.2f,
                Radius = baseUnit.Radius * 1.5f,
            };

            // We need a more elegant way to get and set damage.
            // Let's assume a better Prototype and Ability structure.
            var eliteAbilities = ImmutableArray.CreateBuilder<AbilityPrototype>();
            foreach(var ability in elitePrototype.Abilities)
            {
                // This part assumes you've refactored AbilityPrototype to not use a switch
                // A better way would be for the ability itself to have a "WithIncreasedDamage" method.
                var modifiedAbility = ability.WithIncreasedDamage(4);
                eliteAbilities.Add(modifiedAbility);
            }

            return elitePrototype with { Abilities = eliteAbilities.ToImmutable() };
        }
    }
}