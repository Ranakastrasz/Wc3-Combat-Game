using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Util
{
    public static class GraphicsUtils
    {
        #region Conversions
        // Not all of these are likely to be useful.
        // But lets me use the correct one from the job, and convert as needed, without worry.
        public static Vector2 ToVector2(this Point value) => new(value.X, value.Y);
        public static Vector2 ToVector2(this Size value) =>
            new(value.Width, value.Height);
        public static Point ToPoint(this Vector2 value) =>
            new((int)Math.Round(value.X), (int)Math.Round(value.Y));
        public static Point ToPoint(this Size value) =>
            new(value.Width, value.Height);
        public static Size ToSize(this Vector2 value) =>
            new((int)Math.Round(value.X), (int)Math.Round(value.Y));
        public static Size ToSize(this Point value) =>
            new(value.X, value.Y);
        public static Vector2 Center(this Rectangle rect) =>
            new(
                rect.X + rect.Width / 2,
                rect.Y + rect.Height / 2
            );
        #endregion


        // Snapping and Rounding Helpers
        // If float precision is causing layout issues:
        #region RoundingHelpers
        public static Point RoundToPoint(this Vector2 value) =>
            new(
                (int)Math.Round(value.X),
                (int)Math.Round(value.Y)
            );

        public static Vector2 Floor(this Vector2 value) =>
            new(
                (float)Math.Floor(value.X),
                (float)Math.Floor(value.Y)
            );

        public static Vector2 Ceil(this Vector2 value) =>
            new(
                (float)Math.Ceiling(value.X),
                (float)Math.Ceiling(value.Y)
            );
        #endregion

        // Rect Creation Utilities
        #region Rectangle
        public static Rectangle RectFromTopLeft(this Vector2 topLeft, Vector2 size)
        {
            if (size.X < 0 || size.Y < 0)
                throw new ArgumentException("Size must have non-negative dimensions", nameof(size));
            return new(
                (int)Math.Floor(topLeft.X),
                (int)Math.Floor(topLeft.Y),
                (int)Math.Ceiling(size.X),
                (int)Math.Ceiling(size.Y)
            );
        }

        public static Rectangle RectFromTopRight(this Vector2 topRight, Vector2 size)
        {
            if (size.X < 0 || size.Y < 0)
                throw new ArgumentException("Size must have non-negative dimensions", nameof(size));
            return new(
                (int)Math.Floor(topRight.X - size.X),
                (int)Math.Floor(topRight.Y),
                (int)Math.Ceiling(size.X),
                (int)Math.Ceiling(size.Y)
            );
        }


        public static Rectangle RectFromBottomLeft(this Vector2 bottomLeft, Vector2 size)
        {
            if (size.X < 0 || size.Y < 0)
                throw new ArgumentException("Size must have non-negative dimensions", nameof(size));
            return new(
                (int)Math.Floor(bottomLeft.X),
                (int)Math.Floor(bottomLeft.Y - size.Y),
                (int)Math.Ceiling(size.X),
                (int)Math.Ceiling(size.Y)
            );
        }

        public static Rectangle RectFromBottomRight(this Vector2 bottomRight, Vector2 size)
        {
            if (size.X < 0 || size.Y < 0)
                throw new ArgumentException("Size must have non-negative dimensions", nameof(size));
            return new(
                (int)Math.Floor(bottomRight.X - size.X),
                (int)Math.Floor(bottomRight.Y - size.Y),
                (int)Math.Ceiling(size.X),
                (int)Math.Ceiling(size.Y)
            );
        }

        public static Rectangle RectFromCenter(this Vector2 center, Vector2 size)
        {
            if (size.X < 0 || size.Y < 0)
                throw new ArgumentException("Size must have non-negative dimensions", nameof(size));

            return new(
                (int)Math.Floor(center.X - size.X / 2),
                (int)Math.Floor(center.Y - size.Y / 2),
                (int)Math.Ceiling(size.X),
                (int)Math.Ceiling(size.Y)
             );
        }

        public static Rectangle RectFromBottomCenter(this Vector2 bottomCenter, Vector2 size)
        {
            if (size.X < 0 || size.Y < 0)
                throw new ArgumentException("Size must have non-negative dimensions", nameof(size));

            return new(
                (int)Math.Floor(bottomCenter.X - size.X / 2),
                (int)Math.Floor(bottomCenter.Y - size.Y),
                (int)Math.Ceiling(size.X),
                (int)Math.Ceiling(size.Y)
            );
        }
        #endregion

        // Inflation and Padding
        // For layout tweaking and spacing:
        #region InflationAndPadding
        public static Rectangle Inflate(this Rectangle rect, int padding) =>
            Rectangle.Inflate(rect, padding, padding);

        public static Rectangle Deflate(this Rectangle rect, int left, int top, int right, int bottom)
        {
            int width = rect.Width - left - right;
            int height = rect.Height - top - bottom;
            if (width < 0 || height < 0)
                throw new ArgumentException("Deflate parameters result in negative dimensions");

            return new Rectangle(rect.X + left, rect.Y + top, width, height);
        }
        #endregion 
        public static bool Contains(this Rectangle rect, Vector2 point) =>
            rect.Contains((int)Math.Floor(point.X), (int)Math.Floor(point.Y));
        public static Vector2 Clamp(this Vector2 value, Rectangle bounds)
        {
            return value.Clamp(
                new Vector2(bounds.Left, bounds.Top),
                new Vector2(bounds.Right - 1, bounds.Bottom - 1)
            );
        }
    }
}
