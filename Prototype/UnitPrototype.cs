using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Prototype
{
    /// <summary>
    /// Represents a Prefab for a living or interactive game unit with health and actions.
    /// 
    /// </summary>
    internal struct UnitPrototype
    {
        public float MaxHealth { get; set; }
        public float HealthRegen {  get; set; }
        public float Size { get; set; }
        public float Speed { get; set; }
        public Brush FillBrush { get; set; }
        public enum DrawShape // Probably don't want it here specificially.
        {
            Square,
            Circle
        }
        public DrawShape Shape { get; set; }

        public UnitPrototype(float maxHealth, float healthRegen, float size, float speed, Brush fillBrush, DrawShape shape)
        {
            MaxHealth = maxHealth;
            HealthRegen = healthRegen;
            Size = size;
            Speed = speed;
            Shape = shape;
            FillBrush = fillBrush;
        }

    }
}
