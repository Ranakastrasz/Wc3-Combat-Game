using System.Collections.Immutable;

namespace Wc3_Combat_Game.GameEngine.Data.Data
{
    /// <summary>
    /// Represents a Prefab for a living or interactive game unit with health and actions.
    /// </summary>
    public record UnitData(
        string ID,
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
        public ImmutableArray<string> Abilities { get; init; } = ImmutableArray<string>.Empty;

        // Method to add an ability, returning a new record instance
        public UnitData AddAbility(string ability) =>
            this with { Abilities = Abilities.Add(ability) };
        public UnitData AddWeaponAndRegister(AbilityData ability)
        {
            if (!DataManager.TryGetAbility(ability.ID, out var existingAbility))
            {
                DataManager.RegisterAbility(ability);
            }
            return AddAbility(ability.ID);
        }
    }
}