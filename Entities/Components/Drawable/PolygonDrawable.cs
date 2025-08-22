using System;
using System.Drawing; // Make sure to include this for Graphics, Color, RectangleF
using System.Drawing.Drawing2D;
using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Entities.Components.Drawable
{
    public class PolygonDrawable: DrawableComponent // Normal polygons only.
    {
        private readonly Func<float> _getDiameter; // Within a square bounding box.
        private readonly Func<int> _getVertexes;
        private readonly Func<float> _getRotation;

        public PolygonDrawable(Func<IDrawContext, Color> getColor, Func<Vector2> getPosition, Func<float> getDiameter, Func<float> getRotation, Func<int> getVertexes, Func<bool> getVisible)
            : base(getColor, getPosition, getVisible)
        {
            _getDiameter = getDiameter;
            _getVertexes = getVertexes;
            _getRotation = getRotation;
        }

        public override void Draw(Graphics g, IDrawContext context)
        {
            if(!_getVisible()) return;

            float diameter = _getDiameter();
            if(diameter <= 0) return;

            float rotation = _getRotation();
            Vector2 position = _getPosition();

            int vertexCount = _getVertexes();
            var brush = context.DrawCache.GetSolidBrush(_getColor(context));
            var pen = context.DrawCache.GetPen(_getColor(context));

            switch(vertexCount)
            {
                case 0: // Or case 1:
                case 1: // A single point or no vertices might imply a circle or nothing.
                    DrawCircle(g, brush, position, diameter);
                    break;
                case 2: // Line segment
                    DrawLine(g, pen, position, rotation, diameter);
                    break;
                case 3: // Triangle
                    DrawRegularPolygon(g, brush, position, rotation, diameter, 3);
                    break;
                case 4: // Rectangle (now works correctly with rotation)
                    DrawSquare(g, brush, position, rotation, diameter);
                    break;
                default: // For 5 to 10 vertices, draw a regular polygon. For 10+, draw a circle.
                    if(vertexCount >= 5 && vertexCount <= 10)
                    {
                        DrawRegularPolygon(g, brush, position, rotation, diameter, vertexCount);
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
            var save = g.Save();
            g.TranslateTransform(center.X, center.Y);
            RectangleF boundingBox = new RectangleF(-diameter / 2, -diameter / 2, diameter, diameter);
            g.FillEllipse(brush, boundingBox);
            g.Restore(save);
        }

        /// <summary>
        /// Helper to draw a line segment.
        /// </summary>
        private void DrawLine(Graphics g, Pen pen, Vector2 center, float rotation, float length)
        {
            var save = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(GeometryUtils.RadToDeg(rotation));

            // The line is now drawn centered on the new origin (0,0)
            float halfLength = length / 2f;
            PointF p1 = new PointF(-halfLength, 0);
            PointF p2 = new PointF(halfLength, 0);
            g.DrawLine(pen, p1, p2); // Lines are drawn with a Pen, not a Brush

            g.Restore(save);
        }

        /// <summary>
        /// Helper to draw a square.
        /// </summary>
        private void DrawSquare(Graphics g, SolidBrush brush, Vector2 center, float rotation, float size)
        {
            var save = g.Save();
            g.TranslateTransform(center.X, center.Y);
            g.RotateTransform(GeometryUtils.RadToDeg(rotation));
            RectangleF boundingBox = new RectangleF(-size / 2, -size / 2, size, size);
            g.FillRectangle(brush, boundingBox);
            g.Restore(save);
        }

        /// <summary>
        /// Helper to draw a regular polygon.
        /// </summary>
         private void DrawRegularPolygon(Graphics g, SolidBrush brush, Vector2 center, float rotation, float diameter, int sides)
        {
            if (sides < 3) return;

            var save = g.Save();
            g.TranslateTransform(center.X, center.Y);

            float radius = diameter / 2f;
            PointF[] points = new PointF[sides];
            double angleIncrement = 2 * Math.PI / sides;

            double startAngle = rotation;// - Math.PI / 2;

            for (int i = 0; i < sides; i++)
            {
                double angle = startAngle + i * angleIncrement;
                points[i] = new PointF(
                    radius * (float)Math.Cos(angle),
                    radius * (float)Math.Sin(angle)
                );
            }
            g.FillPolygon(brush, points);

            g.Restore(save);
        }
    }
}
