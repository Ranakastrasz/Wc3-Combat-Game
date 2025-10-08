using System.Collections.Immutable;

using AssertUtils;

using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.GameEngine.Actions.Interface;

namespace Wc3_Combat_Game.Entities.Projectiles.Prototypes
{
    public struct ProjectilePrototype
    {

        public float Radius;
        public float Speed;
        public float Lifespan;
        public bool PierceUnits; // May need terrain vs unit piercing later.

        public ImmutableArray<IGameplayAction> ImpactActions;
        public Color Color;


        private ProjectilePrototype(float radius, float speed, float lifespan, ImmutableArray<IGameplayAction> impactActions, Color color)
        {
            Radius = radius;
            Speed = speed;
            Lifespan = lifespan;
            ImpactActions = impactActions;
            Color = color;
        }

        public ProjectilePrototype(float radius, float speed, float lifespan, IGameplayAction? impactAction, int vertextCount, Color color)
            : this(radius, speed, lifespan,
                  impactAction != null ? ImmutableArray.Create(impactAction) : ImmutableArray<IGameplayAction>.Empty,
                  color)
        {
        }

        public ProjectilePrototype() 
        {
            Radius = 0;
            Speed = 0;
            Lifespan = 0;
            ImpactActions = ImmutableArray<IGameplayAction>.Empty;
            Color = Color.White;
        }

        public ProjectilePrototype AddImpactAction(IGameplayAction impactAction)
        {
            return new ProjectilePrototype(Radius, Speed, Lifespan, ImpactActions.Add(impactAction), Color);
        }
    }
    
}
