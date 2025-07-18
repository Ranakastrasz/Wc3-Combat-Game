﻿using System.Numerics;

using AStar;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Entities.Components
{
    internal class CircleCollider: ICollidable
    {
        private float _radius;
        private bool _sweptCollision;

        //public event EventHandler TerrainCollision;

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

        public bool CollidesAt(Entity owner, IBoardContext context)
        {
            return context.Map.CollidesAt(owner.Position, _radius);
        }
        public bool CollidesAt(Vector2 position, IBoardContext context)
        {
            return context.Map.CollidesAt(position, _radius);
        }
    }
}