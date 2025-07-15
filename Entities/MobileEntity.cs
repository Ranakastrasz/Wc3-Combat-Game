using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities
{
    public class MobileEntity: Entity
    {
        protected Vector2 _velocity = Vector2.Zero;
        public MobileEntity(float size, Vector2 position) : base(size, position)
        {

        }

        public override void Update(float deltaTime, IBoardContext context)
        {
            Vector2 newPosition = _position + _velocity * deltaTime;
            Map? map = context.Map;
            AssertUtil.NotNull(map);

            float collisionRadius = Radius;

            // Try full movement first
            if(map.HasLineOfSight(_position, newPosition, collisionRadius))
            {
                _position = newPosition;
                return;
            }

            // Try X-only
            Vector2 xOnly = _position + new Vector2(_velocity.X * deltaTime, 0);
            bool xOk = map.HasLineOfSight(_position, xOnly, collisionRadius);

            // Try Y-only
            Vector2 yOnly = _position + new Vector2(0, _velocity.Y * deltaTime);
            bool yOk = map.HasLineOfSight(_position, yOnly, collisionRadius);

            if(xOk)
                _position += new Vector2(_velocity.X * deltaTime, 0);
            else if(yOk)
                _position += new Vector2(0, _velocity.Y * deltaTime);
            else
            { } // Cannot move.
            //AssertUtil.Assert(map.CollidesAt(_position, collisionRadius) == false, "Post-collision position is colliding!");

            OnTerrainCollision(context);

        }

        protected virtual void OnTerrainCollision(IBoardContext context)
        {
            // Default: stop movement
            _velocity = Vector2.Zero;
        }

        internal new void DrawDebug(Graphics g, IDrawContext context)
        {
            base.DrawDebug(g, context);
            if(context.DebugSettings.Get(DebugSetting.DrawEntityMovementVector))
            {
                if(_velocity != Vector2.Zero)
                {

                }
                var pen = context.DrawCache.GetPen(Color.Lime,2);
                var endPoint = _position + _velocity*0.1f;
                g.DrawLine(pen, _position.ToPointF(), endPoint.ToPointF());
            }
        }
    }
}
