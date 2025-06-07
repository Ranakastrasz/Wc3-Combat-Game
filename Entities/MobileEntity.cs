using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Entities
{
    public class MobileEntity : Entity
    {
        protected Vector2 _velocity = Vector2.Zero;
        public MobileEntity(float size, Vector2 position, Color color): base(size,position,color)
        { 
            
        }

        public override void Update(float deltaTime, IBoardContext context)
        {
            Vector2 newPosition = _position + _velocity * deltaTime;
            Map map = context.Map;
            float collisionRadius = CollisionRadius;

            if (!map.CollidesAt(newPosition, collisionRadius))
            {
                _position = newPosition;
            }
            else
            {
                float moveX = _velocity.X * deltaTime;
                float moveY = _velocity.Y * deltaTime;


                float xDirection = Math.Sign(moveX);
                float allowedMoveX = 0;
                if (moveX != 0)
                {
                    float step = 0.1f * xDirection;
                    for (float delta = 0; Math.Abs(delta) < Math.Abs(moveX); delta += step)
                    {
                        Vector2 testPos = _position + new Vector2(delta, 0);
                        if (map.CollidesAt(testPos, collisionRadius))
                            break;
                        allowedMoveX = delta;
                    }
                }

                float yDirection = Math.Sign(moveY);
                float allowedMoveY = 0;
                if (moveY != 0)
                {
                    float step = 0.1f * yDirection;
                    for (float delta = 0; Math.Abs(delta) < Math.Abs(moveY); delta += step)
                    {
                        Vector2 testPos = _position + new Vector2(0, delta);
                        if (map.CollidesAt(testPos, collisionRadius))
                            break;
                        allowedMoveY = delta;
                    }
                }
                _position += new Vector2(allowedMoveX, allowedMoveY);
                OnTerrainCollision(context);
            }

        }

        protected virtual void OnTerrainCollision(IBoardContext context)
        {
            // Default: stop movement
            _velocity = Vector2.Zero;
        }
    }
}
