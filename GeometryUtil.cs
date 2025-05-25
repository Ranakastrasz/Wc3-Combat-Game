using System;
using System.Numerics;
using System.Drawing;

namespace Wc3_Combat_Game
{
    public static class GeometryUtil
    {


        // Hit Testing / Containment
        // Simplified checks for mouse/UI interaction:
        public static Vector2 Clamp(this Vector2 value, Vector2 min, Vector2 max)
        {
            if (max.X-min.X < 0 || max.Y - min.Y < 0)
                throw new ArgumentException("Bounds rectangle must have non-negative size");
            return new Vector2(
                MathF.Max(min.X, MathF.Min(max.X, value.X)),
                MathF.Max(min.Y, MathF.Min(max.Y, value.Y))
            );
        }
        #region VectorManipulation
        public static Vector2 NormalizeAndScale(this Vector2 vector, float scale) =>
            vector == Vector2.Zero ? Vector2.Zero : Vector2.Normalize(vector) * scale;

        public static float DistanceTo(this Vector2 from, Vector2 to) =>
            Vector2.Distance(from, to);

        public static float AngleTo(this Vector2 from, Vector2 to) =>
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
