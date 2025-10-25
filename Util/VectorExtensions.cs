using System.Numerics;

namespace Wc3_Combat_Game.Util
{
    public static class PointFExtensions
    {
        public static PointF Add(this PointF a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
        public static PointF Subtract(this PointF a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);

    }
    public static class Vector2Extensions
    {
        public static bool IsNaN(this Vector2 v) =>
            float.IsNaN(v.X) || float.IsNaN(v.Y);
    }
}
