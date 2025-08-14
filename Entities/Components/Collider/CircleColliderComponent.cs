using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities.Components.Nebula;

namespace Wc3_Combat_Game.Entities.Components.Collider
{
    public class CircleColliderComponent
    {
        private Fixture _fixture;
        private float _radius;

        public CircleColliderComponent(PhysicsBodyComponent bodyComponent, float radius)
        {
            _radius = radius;
            _fixture = bodyComponent.Body.CreateCircle(radius * GameConstants.PHYSICS_SCALE, 1f);
        }

        public float Radius => _radius;

        public float CollisionRadius => _fixture.Shape.Radius / GameConstants.PHYSICS_SCALE;
    }
}