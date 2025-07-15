using System.Numerics;

using Wc3_Combat_Game.Components.Drawable;
using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Entities.Components
{
    public class CircleDrawable: DrawableComponent
    {
        private readonly Func<Vector2> _getPosition;
        private readonly Func<float> _getRadius;

        public CircleDrawable(Color color, Func<Vector2> getPosition, Func<float> getRadius)
            : base(color)
        {
            _getPosition = getPosition;
            _getRadius = getRadius;
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            // Similar drawing logic for a circle
            Vector2 position = _getPosition();
            float radius = _getRadius();
            float diameter = radius * 2f;
            RectangleF boundingBox = new RectangleF(position.X - radius, position.Y - radius, diameter, diameter);

            var brush = context.DrawCache.GetSolidBrush(_drawColor);
            g.FillEllipse(brush, boundingBox);
        }
    }
}