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
        public float Radius;
        public float Speed;
        public float Lifespan;
        public IGameplayAction? ImpactEffect;
        public Color FillColor;


        public ProjectilePrototype(float size, float speed, float lifespan, IGameplayAction? impactEffect, Color fillColor)
        { 
            Radius = size;
            Speed = speed;
            Lifespan = lifespan;
            ImpactEffect = impactEffect;
            FillColor = fillColor;
        }

        public static ProjectilePrototype Clone(ProjectilePrototype projectile)
        {
            AssertUtil.NotNull(projectile);
            return new ProjectilePrototype(projectile.Radius, projectile.Speed, projectile.Lifespan, projectile.ImpactEffect, projectile.FillColor);
        }
    }
}
