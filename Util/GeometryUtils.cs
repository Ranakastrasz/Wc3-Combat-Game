using System.Numerics;

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
            if(max.X - min.X < 0 || max.Y - min.Y < 0)
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
            return distSq < circleRadius * circleRadius + 0.01f;
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
        /// Checks if a capsule (line segment with radius) intersects an Axis-Aligned Bounding Box (AABB).
        /// A capsule is defined by its two endpoints and a radius.
        /// </summary>
        /// <param name="p1">The start point of the line segment.</param>
        /// <param name="p2">The end point of the line segment.</param>
        /// <param name="radius">The radius of the capsule.</param>
        /// <param name="aabbMin">The minimum corner of the AABB.</param>
        /// <param name="aabbMax">The maximum corner of the AABB.</param>
        /// <returns>True if the capsule intersects the AABB, false otherwise.</returns>
        public static bool CollidesCapsuleWithRectangle(Vector2 p1, Vector2 p2, float radius, Vector2 aabbMin, Vector2 aabbMax)
        {
            // 1. Check if either endpoint's circle overlaps the AABB
            if(CollidesCircleWithRectangle(p1, radius, aabbMin, aabbMax) ||
                CollidesCircleWithRectangle(p2, radius, aabbMin, aabbMax))
            {
                return true;
            }

            // 2. Check if the line segment (p1 to p2) is close enough to any of the AABB's edges.
            // This involves finding the closest point on the line segment to the AABB.
            // If the distance from this closest point to the AABB is less than or equal to the radius,
            // there's a collision.

            // A simpler way to think about this part is checking if the "core" line segment
            // (p1 to p2) intersects the AABB if the AABB itself were "inflated" *only along its normals*.
            // However, the most robust way is to find the closest point on the line segment to the rectangle.

            // Let's implement finding the closest point on the rectangle to the line segment.
            // Or, more commonly, find the closest point on the line segment to the rectangle.

            // Get the closest point on the AABB to the line segment p1-p2
            Vector2 closestPointOnAABB = ClampPointToAABB(GetClosestPointOnLineSegment(p1, p2, GetCenter(aabbMin, aabbMax)), aabbMin, aabbMax);

            // Get the closest point on the line segment to the AABB
            Vector2 closestPointOnSegment = GetClosestPointOnLineSegment(p1, p2, closestPointOnAABB);

            // If the distance between the closest point on the segment and the AABB
            // is less than or equal to the radius, then there's a collision.
            // The distance from closestPointOnSegment to the AABB is essentially just the distance
            // from closestPointOnSegment to closestPointOnAABB, if closestPointOnSegment is outside the AABB.
            // If closestPointOnSegment is inside the AABB, the distance is 0, so it collides.

            // This is the core logic for the "swept body" part of the capsule.
            float dx = Math.Max(0, Math.Max(aabbMin.X - p1.X, p2.X - aabbMax.X));
            float dy = Math.Max(0, Math.Max(aabbMin.Y - p1.Y, p2.Y - aabbMax.Y));

            // Project segment onto X and Y axes
            float segMinX = Math.Min(p1.X, p2.X);
            float segMaxX = Math.Max(p1.X, p2.X);
            float segMinY = Math.Min(p1.Y, p2.Y);
            float segMaxY = Math.Max(p1.Y, p2.Y);

            // Check if the segment crosses the AABB after 'inflating' the AABB by the radius,
            // but in a more precise way than just expanding the AABB directly.
            // This is essentially checking for overlap of the segment's AABB (plus radius) with the tile.
            // A more precise "line segment vs AABB with radius" check:

            // This approach is more robust: Find the closest point on the AABB to the line segment.
            // Then find the closest point on the line segment to *that* closest point on the AABB.
            // If the distance between the closest point on the line segment and the AABB is <= radius, it's a hit.
            // This is a common and reliable method.

            Vector2 centerRect = (aabbMin + aabbMax) / 2f;
            Vector2 halfExtents = (aabbMax - aabbMin) / 2f;

            // Translate segment and AABB so AABB is centered at origin
            Vector2 localP1 = p1 - centerRect;
            Vector2 localP2 = p2 - centerRect;

            Vector2 segmentDir = localP2 - localP1;
            float segmentLengthSq = segmentDir.LengthSquared();

            // Find the closest point on the line segment to the origin (of the translated AABB)
            // using projection.
            float t = 0;
            if(segmentLengthSq > 0)
            {
                t = Vector2.Dot(-localP1, segmentDir) / segmentLengthSq;
                t = Math.Clamp(t, 0f, 1f); // Clamp t to [0, 1] to stay within the segment
            }

            Vector2 closestPointOnLine = localP1 + segmentDir * t;

            // Now, check the distance from this closest point to the AABB's surface.
            // Since the AABB is now centered at the origin, its bounds are -halfExtents to +halfExtents.
            Vector2 clampedClosestPoint = new(
        Math.Clamp(closestPointOnLine.X, -halfExtents.X, halfExtents.X),
        Math.Clamp(closestPointOnLine.Y, -halfExtents.Y, halfExtents.Y)
    );

            float distanceSq = (closestPointOnLine - clampedClosestPoint).LengthSquared();

            if(distanceSq <= radius * radius)
            {
                return true;
            }

            return false;
        }

        // You will need these helper functions inside GeometryUtils:

        /// <summary>
        /// Clamps a point to be within the bounds of an AABB.
        /// This essentially finds the closest point *within* the AABB to the given point.
        /// </summary>
        private static Vector2 ClampPointToAABB(Vector2 point, Vector2 aabbMin, Vector2 aabbMax)
        {
            return new Vector2(
                Math.Clamp(point.X, aabbMin.X, aabbMax.X),
                Math.Clamp(point.Y, aabbMin.Y, aabbMax.Y)
            );
        }

        /// <summary>
        /// Finds the closest point on a line segment to a given point.
        /// </summary>
        private static Vector2 GetClosestPointOnLineSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point)
        {
            Vector2 segmentVector = segmentEnd - segmentStart;
            float segmentLengthSq = segmentVector.LengthSquared();

            if(segmentLengthSq == 0) // It's a point, not a segment
            {
                return segmentStart;
            }

            // Project 'point' onto the line defined by the segment
            float t = Vector2.Dot(point - segmentStart, segmentVector) / segmentLengthSq;

            // Clamp t to [0, 1] to ensure the closest point is on the segment itself
            t = Math.Clamp(t, 0f, 1f);

            return segmentStart + t * segmentVector;
        }

        private static Vector2 GetCenter(Vector2 min, Vector2 max)
        {
            return (min + max) / 2f;
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

        internal static bool CollidesCircleWithCircle(Vector2 positionA, Vector2 positionB, float radiusA, float radiusB)
        {
            // Calculate the squared distance between the two circle centers
            float distanceSquared = (positionB - positionA).LengthSquared();

            // Calculate the sum of the radii
            float sumOfRadii = radiusA + radiusB;

            // Compare the squared distance to the squared sum of the radii
            return distanceSquared <= (sumOfRadii * sumOfRadii);
        }

        internal static Vector2 GetCircleCircleSeperationVector(Vector2 positionA, Vector2 positionB, float radiusA, float radiusB)
        {
            Vector2 distanceVector = positionB - positionA;
            float distance = distanceVector.Length();
            float sumOfRadii = radiusA + radiusB;

            // If the circles are not overlapping, return Vector2.Zero
            if(distance >= sumOfRadii)
                return Vector2.Zero;

            // Calculate the overlap amount
            float overlap = sumOfRadii - distance;

            // Normalize the distance vector to get the direction of overlap
            // Handle the case where circles are perfectly concentric to avoid division by zero
            if(distance == 0)
            {
                // If circles are at the exact same position, define a default separation direction, e.g., along X-axis
                return new Vector2(overlap, 0); // Or any arbitrary non-zero vector with magnitude 'overlap'
            }

            // Multiply the normalized vector by the overlap amount to get the separation vector
            Vector2 separationVector = (distanceVector / distance) * overlap;

            return separationVector;
        }
        #endregion
    }
}
