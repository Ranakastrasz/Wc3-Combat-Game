using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Wc3_Combat_Game.Util
{
    public class Vector2Int
    {
        public int X;
        public int Y;


        public Vector2Int(int x, int y)
        { 
            X = x;
            Y = y;
        }
        public Vector2 ToVector2() => new(X, Y);

        public static Vector2Int operator +(Vector2Int a, Vector2Int b) => new Vector2Int(a.X + b.X, a.Y + b.Y);
        public static Vector2Int operator -(Vector2Int a, Vector2Int b) => new Vector2Int(a.X - b.X, a.Y - b.Y);

        // Length (as a float, similar to Vector2)
        public float Length() => (float)Math.Sqrt(X * X + Y * Y);

        // Length squared (integer result)
        public int LengthSquared() => X * X + Y * Y;

        // Manhattan Distance to another Vector2Int
        public int ManhattanDistance(Vector2Int other)
        {
            return Math.Abs(X - other.X) + Math.Abs(Y - other.Y);
        }

        // Static method for Manhattan Distance
        public static int ManhattanDistance(Vector2Int a, Vector2Int b)
        {
            return Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        }
    }
}
