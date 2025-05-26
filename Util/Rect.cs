using System.Numerics;

namespace Wc3_Combat_Game.Util
{
    public struct Rect
    {
        private Vector2 _center;
        private Vector2 _size;

        // Properties
        public Vector2 Center
        {
            readonly get => _center;
            set => _center = value;
        }

        public Vector2 Size
        {
            readonly get => _size;
            set
            {
                _size = value;
                _halfSize = _size * 0.5f;
            }
        }

        // Cached half-size for calculations
        private Vector2 _halfSize;
        // Derived properties
        public readonly Vector2 Min => _center - _halfSize;
        public readonly Vector2 Max => _center + _halfSize;

        public Rect(Vector2 center, Vector2 size)
        {
            _center = center;
            _size = size;
            _halfSize = size * 0.5f;
        }


        // Create from two corners (min and max)
        public static Rect FromCorners(Vector2 corner1, Vector2 corner2)
        {
            Vector2 min = Vector2.Min(corner1, corner2);
            Vector2 max = Vector2.Max(corner1, corner2);
            Vector2 center = (min + max) * 0.5f;
            Vector2 size = max - min;
            return new Rect(center, size);
        }

        public static Rect FromCenter(Vector2 center, Vector2 size)
        {
            return new Rect(center, size);
        }

        // Checks if a point is inside (inclusive)
        public readonly bool Contains(Vector2 point)
        {
            Vector2 min = Min;
            Vector2 max = Max;
            return point.X >= min.X && point.X <= max.X &&
                    point.Y >= min.Y && point.Y <= max.Y;
        }

        // Checks if this rect fully contains another rect
        public readonly bool Contains(Rect other)
        {
            return Contains(other.Min) && Contains(other.Max);
        }

        // Checks if this rect intersects with another rect
        public readonly bool Intersects(Rect other)
        {
            Vector2 aMin = Min;
            Vector2 aMax = Max;
            Vector2 bMin = other.Min;
            Vector2 bMax = other.Max;

            bool noOverlap = aMax.X < bMin.X || aMin.X > bMax.X ||
                                aMax.Y < bMin.Y || aMin.Y > bMax.Y;

            return !noOverlap;
        }
        public static bool operator ==(Rect a, Rect b) => a.Center == b.Center && a.Size == b.Size;
        public static bool operator !=(Rect a, Rect b) => !(a == b);
        public static Rect operator +(Rect rect, Vector2 offset)
        {
            return new Rect(rect.Center + offset, rect.Size);
        }

        public static Rect operator -(Rect rect, Vector2 offset)
        {
            return new Rect(rect.Center - offset, rect.Size);
        }

        public readonly Rect Union(Rect other)
        {
            Vector2 min = Vector2.Min(Min, other.Min);
            Vector2 max = Vector2.Max(Max, other.Max);
            return FromCorners(min, max);
        }

        public readonly Rect? Intersection(Rect other)
        {
            Vector2 min = Vector2.Max(Min, other.Min);
            Vector2 max = Vector2.Min(Max, other.Max);

            if (min.X > max.X || min.Y > max.Y)
                return null; // No intersection

            return FromCorners(min, max);
        }
        public readonly Rect Scale(float factor, Vector2 pivot)
        {
            // Translate rect so pivot is origin
            Vector2 offset = Center - pivot;
            Vector2 scaledOffset = offset * factor;
            Vector2 newCenter = pivot + scaledOffset;
            Vector2 newSize = Size * factor;
            return new Rect(newCenter, newSize);
        }

        // Convenience overload: scale around center
        public readonly Rect Scale(float factor) => Scale(factor, Center);

        public override readonly string ToString()
            => $"Rect(Center={_center}, Size={_size})";
        public readonly override bool Equals(object? obj)
        {
            if (obj is Rect other)
                return this == other;
            return false;
        }

        public readonly bool Equals(Rect other)
        {
            return this == other;
        }

        public readonly override int GetHashCode()
        {
            // Combine hash codes of center and size
            return HashCode.Combine(Center, Size);
        }
    }
}
