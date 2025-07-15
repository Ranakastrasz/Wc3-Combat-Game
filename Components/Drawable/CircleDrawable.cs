using System.Numerics;

using Wc3_Combat_Game.Components.Drawable;
using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Entities.Components
{
    public class CircleDrawable: DrawableComponent
    {
        private readonly Func<float> _getRadius;

        public CircleDrawable(Func<IDrawContext, Color> getColor, Func<Vector2> getPosition, Func<float> getRadius, Func<bool> getVisible)
            : base(getColor, getPosition, getVisible)
        {
            _getRadius = getRadius;
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            if(!_getVisible()) return;
            Vector2 position = _getPosition();
            float radius = _getRadius();
            float diameter = radius * 2f;
            RectangleF boundingBox = new RectangleF(position.X - radius, position.Y - radius, diameter, diameter);

            var brush = context.DrawCache.GetSolidBrush(_getColor(context));
            g.FillEllipse(brush, boundingBox);
        }
    }
}