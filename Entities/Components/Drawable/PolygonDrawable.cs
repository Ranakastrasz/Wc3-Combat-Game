using System.Drawing; // Make sure to include this for Graphics, Color, RectangleF
using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities.Components.Drawable
{
    public class PolygonDrawable: DrawableComponent // Normal polygons only.
    {
        private readonly Func<float> _getDiameter; // Within a square bounding box.
        private readonly Func<int> _getVertexes;

        public PolygonDrawable(Func<IDrawContext, Color> getColor, Func<Vector2> getPosition, Func<float> getDiameter, Func<int> getVertexes, Func<bool> getVisible)
            : base(getColor, getPosition, getVisible)
        {
            _getDiameter = getDiameter;
            _getVertexes = getVertexes;
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            // Optional: Debug settings check
            //if (!context.DebugSettings.Get(DebugSetting.DrawEntities))
            //{
            //    return;
            //}
            if(!_getVisible()) return;

            float diameter = _getDiameter();
            if(diameter <= 0) return;

            int vertexCount = _getVertexes();
            Vector2 position = _getPosition();
            var brush = context.DrawCache.GetSolidBrush(_getColor(context));
            var pen = context.DrawCache.GetPen(_getColor(context));

            switch(vertexCount)
            {
                case 0: // Or case 1:
                case 1: // A single point or no vertices might imply a circle or nothing.
                    DrawCircle(g, brush, position, diameter);
                    break;
                case 2: // Line segment
                    DrawLine(g, pen, position, diameter);
                    break;
                case 3: // Triangle
                    DrawRegularPolygon(g, brush, position, diameter, 3);
                    break;
                case 4: // Rectangle (you'll need to consider rotation)
                    DrawRectangle(g, brush, position, diameter);
                    break;
                default: // For 5 to 10 vertices, draw a regular polygon. For 10+, draw a circle.
                    if(vertexCount >= 5 && vertexCount <= 10)
                    {
                        DrawRegularPolygon(g, brush, position, diameter, vertexCount);
                    }
                    else // For 10+ vertices, draw a circle as an approximation.
                    {
                        DrawCircle(g, brush, position, diameter);
                    }
                    break;
            }
        }

        /// <summary>
        /// Helper to draw a circle.
        /// </summary>
        private void DrawCircle(Graphics g, SolidBrush brush, Vector2 center, float diameter)
        {
            RectangleF boundingBox = new RectangleF(center.X - diameter / 2, center.Y - diameter / 2, diameter, diameter);
            g.FillEllipse(brush, boundingBox);
        }

        /// <summary>
        /// Helper to draw a line segment. This will require more context, such as start/end points, or a way to define its length and angle relative to the center. For now, it's a placeholder.
        /// </summary>
        private void DrawLine(Graphics g, Pen pen, Vector2 center, float length)
        {
            // To draw a line, you typically need two points.
            // If length is the diameter, it might represent a line through the center.
            // This is a simplified placeholder. You'd need more data (e.g., rotation).
            float halfLength = length / 2f;
            PointF p1 = new PointF(center.X - halfLength, center.Y);
            PointF p2 = new PointF(center.X + halfLength, center.Y);
            g.DrawLine(pen, p1, p2); // Lines are drawn with a Pen, not a Brush
        }

        /// <summary>
        /// Helper to draw a rectangle. Currently, it draws an axis-aligned rectangle.
        /// For rotation, you'd need to apply a transformation to the Graphics object or calculate rotated points.
        /// </summary>
        private void DrawRectangle(Graphics g, SolidBrush brush, Vector2 center, float size)
        {
            Vector2 boundingSize = new Vector2(size, size);
            // Assuming RectFFromCenter is an extension method you have
            RectangleF boundingBox = center.RectFFromCenter(boundingSize);
            g.FillRectangle(brush, boundingBox);
        }

        /// <summary>
        /// Helper to draw a regular polygon.
        /// </summary>
        private void DrawRegularPolygon(Graphics g, SolidBrush brush, Vector2 center, float diameter, int sides)
        {
            if(sides < 3) return; // A polygon needs at least 3 sides.

            float radius = diameter / 2f;
            PointF[] points = new PointF[sides];
            double angleIncrement = 2 * Math.PI / sides;

            // Start angle to orient the polygon. For a square, 45 degrees rotates it.
            // For a general polygon, you might want 0 or -PI/2 (to have a flat top/bottom).
            double startAngle = -Math.PI / 2; // Start from the top

            for(int i = 0; i < sides; i++)
            {
                double angle = startAngle + i * angleIncrement;
                points[i] = new PointF(
                    center.X + radius * (float)Math.Cos(angle),
                    center.Y + radius * (float)Math.Sin(angle)
                );
            }
            g.FillPolygon(brush, points);
        }
    }
}