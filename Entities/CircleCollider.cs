using System.Numerics;

using AStar;

using Wc3_Combat_Game.Components.Entitys;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Entities
{
    internal class CircleCollider: ICollidable
    {
        private float _radius;
        private bool _sweptCollision;

        public event EventHandler TerrainCollision;

        public float CollisionRadius => _radius;
        public CircleCollider(float radius, bool sweptCollision = false)
        {
            _radius = radius;
            _sweptCollision = sweptCollision;
        }


        public bool HasClearPathTo(Vector2 position, Vector2 targetPosition, IBoardContext context)
        {
            if(_sweptCollision)
            {
                return context.Map.HasLineOfSight(position, targetPosition, CollisionRadius);
            }
            else
            {
                return !CollidesAt(targetPosition, context);
            }
        }
        public void OnTerrainCollision(Entity owner, IBoardContext context)
        {
            owner.OnTerrainCollision(context);
        }

        public void Update(Entity owner, float deltaTime, IBoardContext context)
        {
            // Does nothing for now.
            // Will include collision checks with other entities and terrain later, mostly for the aggressive push effect.
        }
    }
}