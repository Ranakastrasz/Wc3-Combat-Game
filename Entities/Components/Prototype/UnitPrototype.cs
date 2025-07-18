using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using System.Drawing;
using System.Collections.Immutable;

namespace Wc3_Combat_Game.Entities.Components.Prototype
{
    /// <summary>
    /// Represents a Prefab for a living or interactive game unit with health and actions.
    /// </summary>
    public readonly struct UnitPrototype
    {
        public readonly float MaxLife;
        public readonly float LifeRegen;
        public readonly float MaxMana;
        public readonly float ManaRegen;
        public readonly float Radius;
        public readonly float Mass;
        public readonly float PushPriority;
        public readonly float Speed;
        public readonly Color Color;
        public readonly Color DamagedColor;
        public readonly Color DeadColor;
        public readonly int PolygonCount;
        public readonly ImmutableArray<AbilityPrototype> Abilities;


        // Private constructor for "with" methods to use
        private UnitPrototype(
            float maxLife, float lifeRegen,
            float maxMana, float manaRegen,
            float radius, float mass,
            float pushPriority, float speed,
            Color color, Color damagedColor, Color deadColor,
            int polygonCount,
            ImmutableArray<AbilityPrototype> weapons)
        {
            MaxLife = maxLife;
            LifeRegen = lifeRegen;
            MaxMana = maxMana;
            ManaRegen = manaRegen;
            Radius = radius;
            Mass = mass;
            PushPriority = pushPriority;
            Speed = speed;
            Color = color;
            DamagedColor = damagedColor;
            DeadColor = deadColor;
            PolygonCount = polygonCount;
            Abilities = weapons;
        }

        // Initial constructor for basic properties
        public UnitPrototype(float maxLife, float lifeRegen, float radius, float speed, Color fillColor, int polygonCount)
            : this(maxLife, lifeRegen, 0f, 0f, radius, radius * radius, radius * radius, speed, fillColor, Color.White, Color.Gray, polygonCount, ImmutableArray<AbilityPrototype>.Empty)
        {
            // The 'this' call handles all field assignments.
        }

        //            return new UnitPrototype(MaxLife, LifeRegen, MaxMana, ManaRegen, Radius, Mass, PushPriority, Speed, Color, DamagedColor, DeadColor, Shape, Weapons);


        // "With" methods for step-by-step construction
        public UnitPrototype WithLife(float maxLife, float lifeRegen)
        {
            return new UnitPrototype(maxLife, lifeRegen, MaxMana, ManaRegen, Radius, Mass, PushPriority, Speed, Color, DamagedColor, DeadColor, PolygonCount, Abilities);
        }
        public UnitPrototype WithMana(float maxMana, float manaRegen)
        {
            return new UnitPrototype(MaxLife, LifeRegen, maxMana, manaRegen, Radius, Mass, PushPriority, Speed, Color, DamagedColor, DeadColor, PolygonCount, Abilities);
        }
        public UnitPrototype WithRadius(float radius)
        {
            return new UnitPrototype(MaxLife, LifeRegen, MaxMana, ManaRegen, radius, radius * radius, radius * radius, Speed, Color, DamagedColor, DeadColor, PolygonCount, Abilities);
        }
        public UnitPrototype WithSpeed(float speed)
        {
            return new UnitPrototype(MaxLife, LifeRegen, MaxMana, ManaRegen, Radius, Mass, PushPriority, speed, Color, DamagedColor, DeadColor, PolygonCount, Abilities);
        }


        public UnitPrototype WithAbilities(AbilityPrototype[] abilities)
        {
            return new UnitPrototype(MaxLife, LifeRegen, MaxMana, ManaRegen, Radius, Mass, PushPriority, Speed, Color, DamagedColor, DeadColor, PolygonCount, abilities.ToImmutableArray<AbilityPrototype>());
        }

        // Example of adding one weapon at a time (if you prefer that granularity)
        public UnitPrototype AddWeapon(AbilityPrototype weapon)
        {
            return new UnitPrototype(MaxLife, LifeRegen, MaxMana, ManaRegen, Radius, Mass, PushPriority, Speed, Color, DamagedColor, DeadColor, PolygonCount, Abilities.Add(weapon));

        }
        // If you want to modify other properties after initial construction
        public UnitPrototype WithColors(Color fillColor, Color damagedColor, Color deadColor, int polygonCount)
        {
            return new UnitPrototype(MaxLife, LifeRegen, MaxMana, ManaRegen, Radius, Mass, PushPriority, Speed, fillColor, damagedColor, deadColor, polygonCount, Abilities);
        }


        //public UnitPrototype WithEliteModifier()
        //{
        //    return new UnitPrototype(Life*4, LifeRegen*4, Mana, ManaRegen, Radius*1.5f, Mass*1.5f, PushPriority*2, Speed*1.1f, Color, DamagedColor, DeadColor, //Shape, Weapons);
        //}
    }
}