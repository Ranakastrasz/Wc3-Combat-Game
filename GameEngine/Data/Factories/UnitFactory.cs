using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Units.Abilities;
using Wc3_Combat_Game.GameEngine.Data.Data;

namespace Wc3_Combat_Game.GameEngine.Data.Factories
{
    public static class UnitFactory
    {

        public static UnitData CreateEliteUnit(UnitData baseUnit)
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
            var eliteAbilities = ImmutableArray.CreateBuilder<string>();
            foreach(var ability in elitePrototype.Abilities)
            {
                // This part assumes you've refactored AbilityPrototype to not use a switch
                // A better way would be for the ability itself to have a "WithIncreasedDamage" method.
                var modifiedAbility = DataManager.TryGetAbility(ability, out var abilityPrototype) && abilityPrototype != null
                    ? abilityPrototype.WithIncreasedDamage(2)
                    : throw new ArgumentException($"No AbilityPrototype found with ID '{ability}'.");

                eliteAbilities.Add(modifiedAbility.ID);
            }

            return elitePrototype with { Abilities = eliteAbilities.ToImmutable() };
        }
    }
}