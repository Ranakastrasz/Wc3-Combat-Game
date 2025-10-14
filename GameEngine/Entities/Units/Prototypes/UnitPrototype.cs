using System.Collections.Immutable;

using Wc3_Combat_Game.Entities.Units.Abilities;
using Wc3_Combat_Game.GameEngine.Data;

namespace Wc3_Combat_Game.Entities.Units.Prototypes
{
    /// <summary>
    /// Represents a Prefab for a living or interactive game unit with health and actions.
    /// </summary>
    public record UnitPrototype(
        string id,
        string name,
        float MaxLife,
        float LifeRegen,
        float Radius,
        float Speed,
        Color Color,
        int PolygonCount)
    {
        // Public properties with default values
        public float MaxMana { get; init; } = 0f;
        public float ManaRegen { get; init; } = 0f;
        public float Mass { get; init; } = Radius * Radius;
        public float PushPriority { get; init; } = Radius * Radius;
        public Color DamagedColor { get; init; } = Color.White;
        public Color DeadColor { get; init; } = Color.Gray;
        public ImmutableArray<string> Abilities { get; init; } = ImmutableArray<string>.Empty;

        // Method to add an ability, returning a new record instance
        public UnitPrototype AddAbility(string ability) =>
            this with { Abilities = Abilities.Add(ability) };
        public UnitPrototype AddWeaponAndRegister(AbilityPrototype ability)
        {
            if (!PrototypeManager.TryGetAbility(ability.ID, out var existingAbility))
            {
                PrototypeManager.RegisterAbility(ability);
            }
            return AddAbility(ability.ID);
        }
    }
}