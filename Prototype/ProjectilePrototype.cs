using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Effects;

namespace Wc3_Combat_Game.Prototype
{
    public class ProjectilePrototype
    {
        public float Size;
        public float Speed;
        public float Lifespan;
        public Effect? ImpactEffect;
        public Brush FillBrush;

        public ProjectilePrototype(float size, float speed, float lifespan, Effect? impactEffect, Brush brush)
        { 
            Size = size;
            Speed = speed;
            Lifespan = lifespan;
            ImpactEffect = impactEffect;
            FillBrush = brush;
        }
    }
}
