using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;

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

        public void Draw(Graphics g, float scale, Brush brush)
        {
            var location = GraphicsUtils.Scale(Position, scale);
            var size = new Size((int)scale, (int)scale);
            var rect = new Rectangle(location, size);
            g.FillRectangle(brush, rect);
        }
    }
}
