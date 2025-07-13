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
        public static float DistanceSquared(Point from, Point to) =>
            Vector2.DistanceSquared(from.ToVector2(), to.ToVector2());

        public static bool Collides(RectangleF a, RectangleF b) =>
            a.Left < b.Right && a.Right > b.Left &&
            a.Top < b.Bottom && a.Bottom > b.Top;

        /// <summary>
        /// Checks if a circle collides with a single axis-aligned rectangle.
        /// </summary>
        /// <param name="circleCenter">The center of the circle.</param>
        /// <param name="circleRadius">The radius of the circle.</param>
        /// <param name="rectMin">The minimum corner of the rectangle.</param>
        /// <param name="rectMax">The maximum corner of the rectangle.</param>
        /// <returns>True if the circle and rectangle intersect, false otherwise.</returns>
        public static bool CollidesCircleWithRectangle(Vector2 circleCenter, float circleRadius, Vector2 rectMin, Vector2 rectMax)
        {
            // Find the closest point on the rectangle to the circle's center.
            Vector2 closestPoint = Vector2.Clamp(circleCenter, rectMin, rectMax);

            // Calculate the squared distance from the circle's center to this closest point.
            float distSq = Vector2.DistanceSquared(circleCenter, closestPoint);

            // Collision occurs if the squared distance is less than the squared radius.
            return distSq < circleRadius * circleRadius;
        }

        /// <summary>
        /// Checks if a line segment intersects an Axis-Aligned Bounding Box (AABB).
        /// This implementation checks if either endpoint is inside the AABB, or if the segment
        /// intersects any of the AABB's four edges.
        /// </summary>
        /// <param name="p1">The start point of the line segment.</param>
        /// <param name="p2">The end point of the line segment.</param>
        /// <param name="aabbMin">The minimum corner of the AABB.</param>
        /// <param name="aabbMax">The maximum corner of the AABB.</param>
        /// <returns>True if the line segment intersects the AABB, false otherwise.</returns>
        public static bool LineSegmentIntersectsAABB(Vector2 p1, Vector2 p2, Vector2 aabbMin, Vector2 aabbMax)
        {
            // Check if either endpoint is inside the AABB
            if((p1.X >= aabbMin.X && p1.X <= aabbMax.X && p1.Y >= aabbMin.Y && p1.Y <= aabbMax.Y) ||
                (p2.X >= aabbMin.X && p2.X <= aabbMax.X && p2.Y >= aabbMin.Y && p2.Y <= aabbMax.Y))
            {
                return true;
            }

            // Check intersection with each of the 4 edges of the AABB
            // Edge 1: Bottom (aabbMin.X, aabbMin.Y) to (aabbMax.X, aabbMin.Y)
            if(LineSegmentIntersectsLineSegment(p1, p2, new Vector2(aabbMin.X, aabbMin.Y), new Vector2(aabbMax.X, aabbMin.Y))) return true;
            // Edge 2: Top (aabbMin.X, aabbMax.Y) to (aabbMax.X, aabbMax.Y)
            if(LineSegmentIntersectsLineSegment(p1, p2, new Vector2(aabbMin.X, aabbMax.Y), new Vector2(aabbMax.X, aabbMax.Y))) return true;
            // Edge 3: Left (aabbMin.X, aabbMin.Y) to (aabbMin.X, aabbMax.Y)
            if(LineSegmentIntersectsLineSegment(p1, p2, new Vector2(aabbMin.X, aabbMin.Y), new Vector2(aabbMin.X, aabbMax.Y))) return true;
            // Edge 4: Right (aabbMax.X, aabbMin.Y) to (aabbMax.X, aabbMax.Y)
            if(LineSegmentIntersectsLineSegment(p1, p2, new Vector2(aabbMax.X, aabbMin.Y), new Vector2(aabbMax.X, aabbMax.Y))) return true;

            return false;
        }

        /// <summary>
        /// Checks if two 2D line segments intersect.
        /// Uses the cross-product method to determine intersection.
        /// </summary>
        /// <param name="p1">Start point of segment 1.</param>
        /// <param name="p2">End point of segment 1.</param>
        /// <param name="p3">Start point of segment 2.</param>
        /// <param name="p4">End point of segment 2.</param>
        /// <returns>True if the segments intersect, false otherwise.</returns>
        public static bool LineSegmentIntersectsLineSegment(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4)
        {
            // Calculate denominators for t and u (parametric equations)
            float den = (p1.X - p2.X) * (p3.Y - p4.Y) - (p1.Y - p2.Y) * (p3.X - p4.X);

            // If denominator is close to zero, lines are parallel or collinear.
            // For game purposes, we generally assume no intersection unless endpoints overlap.
            // A more robust solution would handle collinear overlap explicitly.
            if(Math.Abs(den) < 0.0001f)
            {
                return false; // Lines are parallel or collinear, assume no intersection for simplicity.
            }

            // Calculate t and u parameters
            float t = ((p1.X - p3.X) * (p3.Y - p4.Y) - (p1.Y - p3.Y) * (p3.X - p4.X)) / den;
            float u = -((p1.X - p2.X) * (p1.Y - p3.Y) - (p1.Y - p2.Y) * (p1.X - p3.X)) / den;

            // If t and u are both between 0 and 1 (inclusive), the segments intersect.
            return t >= 0 && t <= 1 && u >= 0 && u <= 1;
        }


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
