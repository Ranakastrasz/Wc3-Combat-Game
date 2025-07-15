namespace Wc3_Combat_Game.Util
{
    internal static class RandomUtils
    {
        private static readonly Random s_Instance = new();

        public static Random Instance => s_Instance;

        public static PointF RandomPointInside(RectangleF rect)
        {
            float x = (float)(rect.Left + RandomUtils.Instance.NextDouble() * rect.Width);
            float y = (float)(rect.Top + RandomUtils.Instance.NextDouble() * rect.Height);
            return new PointF(x, y);
        }

        public static PointF RandomPointBorder(RectangleF rect)
        {
            float width = rect.Width;
            float height = rect.Height;
            float left = rect.Left;
            float top = rect.Top;
            float right = rect.Right;
            float bottom = rect.Bottom;

            float perimeter = 2 * (width + height);
            float t = (float)(RandomUtils.Instance.NextDouble() * perimeter);

            if(t < width)
            {
                // Top edge (left → right)
                return new PointF(left + t, top);
            }
            t -= width;

            if(t < height)
            {
                // Right edge (top → bottom)
                return new PointF(right, top + t);
            }
            t -= height;

            if(t < width)
            {
                // Bottom edge (right → left)
                return new PointF(right - t, bottom);
            }
            t -= width;

            // Left edge (bottom → top)
            return new PointF(left, bottom - t);
        }
        public static PointF RandomPointOutside(RectangleF rect, float padding)
        {
            // Define the outer bounds
            float left = rect.Left - padding;
            float right = rect.Right + padding;
            float top = rect.Top - padding;
            float bottom = rect.Bottom + padding;

            // Choose which side to spawn on: 0=top,1=bottom,2=left,3=right
            int side = RandomUtils.Instance.Next(0, 4);

            switch(side)
            {
                case 0: // top
                    return new PointF(
                        (float)(left + RandomUtils.Instance.NextDouble() * (right - left)),
                        top
                    );
                case 1: // bottom
                    return new PointF(
                        (float)(left + RandomUtils.Instance.NextDouble() * (right - left)),
                        bottom
                    );
                case 2: // left
                    return new PointF(
                        left,
                        (float)(top + RandomUtils.Instance.NextDouble() * (bottom - top))
                    );
                case 3: // right
                    return new PointF(
                        right,
                        (float)(top + RandomUtils.Instance.NextDouble() * (bottom - top))
                    );
                default:
                    throw new Exception("RandomPointOutside bad side");
            }
        }

        internal static int RandomIntBelow(int count)
        {
            return RandomUtils.Instance.Next(0, count);
        }
    }
}
