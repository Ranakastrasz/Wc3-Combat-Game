using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Effects;

namespace Wc3_Combat_Game.Prototypes
{
    internal class ProjectilePrototype
    {
        internal float Size;
        internal float Speed;
        internal float Lifespan;
        internal Effect ImpactEffect;
        internal Brush FillBrush;

        public ProjectilePrototype(float size, float speed, float lifespan, Effect impactEffect, Brush brush)
        { 
            Size = size;
            Speed = speed;
            Lifespan = lifespan;
            ImpactEffect = impactEffect;
            FillBrush = brush;
        }
    }
}
