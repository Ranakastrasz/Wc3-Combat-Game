using AssertUtils;

using Wc3_Combat_Game.Actions.Interface;

namespace Wc3_Combat_Game.Entities.Components.Prototype
{
    public class ProjectilePrototype
    {
        public string Name;
        public float Radius;
        public float Speed;
        public float Lifespan;
        public IGameplayAction? ImpactEffect;
        public Color Color;


        public ProjectilePrototype(string name, float radius, float speed, float lifespan, IGameplayAction? impactEffect, Color color)
        {
            Name = name;
            Radius = radius;
            Speed = speed;
            Lifespan = lifespan;
            ImpactEffect = impactEffect;
            Color = color;
        }

        public static ProjectilePrototype Clone(ProjectilePrototype projectile)
        {
            AssertUtil.NotNull(projectile);
            return new ProjectilePrototype(projectile.Name, projectile.Radius, projectile.Speed, projectile.Lifespan, projectile.ImpactEffect, projectile.Color);
        }
    }
}
