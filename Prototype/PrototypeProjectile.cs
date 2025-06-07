using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Effects;

namespace Wc3_Combat_Game.Prototype
{
    public class PrototypeProjectile
    {
        public float Size;
        public float Speed;
        public float Lifespan;
        public Effects.Action? ImpactEffect;
        public Color FillColor;

        public PrototypeProjectile(float size, float speed, float lifespan, Effects.Action? impactEffect, Color fillColor)
        { 
            Size = size;
            Speed = speed;
            Lifespan = lifespan;
            ImpactEffect = impactEffect;
            FillColor = fillColor;
        }
    }
}
