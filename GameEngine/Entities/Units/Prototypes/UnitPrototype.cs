using System.Collections.Immutable;

using Wc3_Combat_Game.Entities.Units.Abilities;

namespace Wc3_Combat_Game.Entities.Units.Prototypes
{
    /// <summary>
    /// Represents a Prefab for a living or interactive game unit with health and actions.
    /// </summary>
    public record UnitPrototype(
        string Id,
        string Name,
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
        public ImmutableArray<AbilityPrototype> Abilities { get; init; } = ImmutableArray<AbilityPrototype>.Empty;

        // Method to add an ability, returning a new record instance
        public UnitPrototype AddAbility(AbilityPrototype weapon) =>
            this with { Abilities = Abilities.Add(weapon) };
    }
}