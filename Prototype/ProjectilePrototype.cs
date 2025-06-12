using AssertUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Components.Actions.Interface;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Prototype
{
    public class ProjectilePrototype
    {
        public float Size;
        public float Speed;
        public float Lifespan;
        public IGameplayAction? ImpactEffect;
        public Color FillColor;


        public ProjectilePrototype(float size, float speed, float lifespan, IGameplayAction? impactEffect, Color fillColor)
        { 
            Size = size;
            Speed = speed;
            Lifespan = lifespan;
            ImpactEffect = impactEffect;
            FillColor = fillColor;
        }

        public static ProjectilePrototype Clone(ProjectilePrototype projectile)
        {
            AssertUtil.AssertNotNull(projectile);
            return new ProjectilePrototype(projectile.Size, projectile.Speed, projectile.Lifespan, projectile.ImpactEffect, projectile.FillColor);
        }
    }
}
