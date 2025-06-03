using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Terrain
{
    public class Tile
    {
        public TileType Type { get; }
        public Point Position { get; } // Optional

        public int X => Position.X;
        public int Y => Position.Y;

        public Tile(TileType type, Point pos)
        {
            Type = type;
            Position = pos;
        }

        public bool IsWalkable => Type.IsWalkable;
        public char GetChar => Type.Ascii;
        public Color GetColor => Type.Color;
    }
}
