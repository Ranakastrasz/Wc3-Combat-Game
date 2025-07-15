using Wc3_Combat_Game.Prototype.Weapons;

namespace Wc3_Combat_Game.Prototype
{
    /// <summary>
    /// Represents a Prefab for a living or interactive game unit with health and actions.
    /// 
    /// </summary>
    public readonly struct UnitPrototype
    {
        public readonly float Life;
        public readonly float LifeRegen;
        public readonly float Mana = 0f;
        public readonly float ManaRegen = 0f;
        public readonly float Radius;
        public readonly float Mass;
        public readonly float PushPriority;
        public readonly float Speed;
        public readonly Color Color;
        public readonly Color DamagedColor;
        public readonly Color DeadColor;
        public readonly WeaponPrototype Weapon;
        public enum DrawShape // Probably don't want it here specificially.
        {
            Square,
            Circle
        }
        public readonly DrawShape Shape;

        public UnitPrototype(WeaponPrototype weapon, float maxHealth, float healthRegen, float radius, float speed, Color fillColor, DrawShape shape)
        {
            Weapon = weapon;
            Life = maxHealth;
            LifeRegen = healthRegen;
            Radius = radius;
            Mass = radius * radius; // Simple mass calculation based on radius (area for 2D).
            PushPriority = Mass;
            Speed = speed;
            Shape = shape;
            Color = fillColor;
            DamagedColor = Color.White;
            DeadColor = Color.Gray;
        }

        public UnitPrototype(WeaponPrototype weapon, float maxHealth, float healthRegen, float mana, float manaRegen, float radius, float speed, Color fillColor, DrawShape shape = DrawShape.Square)
            : this(weapon, maxHealth, healthRegen, radius, speed, fillColor, shape)
        {
            Mana = mana;
            ManaRegen = manaRegen;
        }
    }
}
