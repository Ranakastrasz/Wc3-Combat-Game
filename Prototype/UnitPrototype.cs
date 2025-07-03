using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public readonly float Size;
        public readonly float Speed;
        public readonly Color FillColor;
        public readonly WeaponPrototype Weapon;
        public enum DrawShape // Probably don't want it here specificially.
        {
            Square,
            Circle
        }
        public readonly DrawShape Shape;

        public UnitPrototype(WeaponPrototype weapon, float maxHealth, float healthRegen, float size, float speed, Color fillColor, DrawShape shape)
        {
            Weapon = weapon;
            Life = maxHealth;
            LifeRegen = healthRegen;
            Size = size;
            Speed = speed;
            Shape = shape;
            FillColor = fillColor;
        }

        public UnitPrototype(WeaponPrototype weapon, float maxHealth, float healthRegen, float mana, float manaRegen, float size, float speed, Color fillColor, DrawShape shape)
            : this(weapon, maxHealth, healthRegen, size, speed, fillColor, DrawShape.Square)
        {
            Mana = mana;
            ManaRegen = manaRegen;
        }
    }
}
