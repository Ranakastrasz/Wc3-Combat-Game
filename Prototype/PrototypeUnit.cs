using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Interface.Weapons;

namespace Wc3_Combat_Game.Prototype
{
    /// <summary>
    /// Represents a Prefab for a living or interactive game unit with health and actions.
    /// 
    /// </summary>
    public readonly struct PrototypeUnit
    {
        public readonly float MaxHealth;
        public readonly float HealthRegen;
        public readonly float Size;
        public readonly float Speed;
        public readonly Color FillColor;
        public readonly PrototypeWeapon Weapon;
        public enum DrawShape // Probably don't want it here specificially.
        {
            Square,
            Circle
        }
        public readonly DrawShape Shape;

        public PrototypeUnit(PrototypeWeapon weapon, float maxHealth, float healthRegen, float size, float speed, Color fillColor, DrawShape shape)
        {
            Weapon = weapon;
            MaxHealth = maxHealth;
            HealthRegen = healthRegen;
            Size = size;
            Speed = speed;
            Shape = shape;
            FillColor = fillColor;
        }
    }
}
