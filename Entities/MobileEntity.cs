using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.Components.Entities;
using Wc3_Combat_Game.Components.Entitys;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities
{
    public class MobileEntity: Entity
    {

        protected Vector2 _velocity
        {
            get => Mover != null ? Mover.Velocity : Vector2.Zero;
            set
            {
                if (Mover != null) Mover.Velocity = value;
            }
        }


        public MobileEntity(float size, Vector2 position) : base(size, position)
        {
            Mover = new UnitMover();
            Collider = new CircleCollider(size, true);
        }

        public override void Update(float deltaTime, IBoardContext context)
        {
            base.Update(deltaTime, context);

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
