using System.Collections.Immutable;

using AssertUtils;

using Wc3_Combat_Game.Actions.Interface;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;

namespace Wc3_Combat_Game.Entities.Components.Prototype
{
    public struct ProjectilePrototype
    {
        public string Name;

        public float Radius;
        public float Speed;
        public float Lifespan;
        public bool PierceUnits; // May need terrain vs unit piercing later.

        public ImmutableArray<IGameplayAction> ImpactActions;
        public Color Color;


        private ProjectilePrototype(string name, float radius, float speed, float lifespan, ImmutableArray<IGameplayAction> impactActions, Color color)
        {
            Name = name;
            Radius = radius;
            Speed = speed;
            Lifespan = lifespan;
            ImpactActions = impactActions;
            Color = color;
        }

        public ProjectilePrototype(string name, float radius, float speed, float lifespan, IGameplayAction impactAction, Color color)
            : this(name, radius, speed, lifespan,
                  ImmutableArray.Create(impactAction),
                  color)
        {

        }

        public ProjectilePrototype AddImpactAction(IGameplayAction impactAction)
        {
            return new ProjectilePrototype(Name, Radius, Speed, Lifespan, ImpactActions.Add(impactAction), Color);
        }
    }
    
}
