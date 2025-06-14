using System;
using System.Numerics;
using System.Drawing;
using System.Collections;

namespace Wc3_Combat_Game.Util
{
    public static partial class GeometryUtils
    {

        public static Vector2 GetPosition(this RectangleF rect) =>
            new Vector2(rect.X, rect.Y);

        public static Vector2 GetSize(this RectangleF rect) =>
            new Vector2(rect.Width, rect.Height);
        
        public static RectangleF RectFromCenter(Vector2 center, Vector2 size) =>
            new(center.X - size.X / 2, center.Y - size.Y / 2, size.X, size.Y);

        // Hit Testing / Containment
        // Simplified checks for mouse/UI interaction:
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
        {
            if (max.X - min.X < 0 || max.Y - min.Y < 0)
                throw new ArgumentException("Bounds rectangle must have non-negative size");
            return new Vector2(
                MathF.Max(min.X, MathF.Min(max.X, value.X)),
                MathF.Max(min.Y, MathF.Min(max.Y, value.Y))
            );
        }
        public static PointF Center(this RectangleF rect)
        {
            return new PointF(rect.Left + rect.Width / 2,
                             rect.Top + rect.Height / 2);
        }

        #region VectorManipulation
        public static Vector2 NormalizeAndScale(Vector2 vector, float scale) =>
            vector == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(vector) * scale;
        
        public static float DistanceTo(Vector2 from, Vector2 to) =>
            Vector2.Distance(from, to);
        public static float DistanceTo(Point from, Point to) =>
            Vector2.Distance(from.ToVector2(), to.ToVector2());
        public static float DistanceToSquared(Vector2 from, Vector2 to) =>
            Vector2.DistanceSquared(from, to);
        public static float DistanceToSquared(Point from, Point to) =>
            Vector2.DistanceSquared(from.ToVector2(), to.ToVector2());

        public static float AngleTo(Vector2 from, Vector2 to) =>
            MathF.Atan2(to.Y - from.Y, to.X - from.X);
        #endregion

        #region Constants
        public static float RadToDeg(this float radians) =>
            radians * (180f / MathF.PI);

        public static float DegToRad(this float degrees) =>
            degrees * (MathF.PI / 180f);
        #endregion
    }
}
