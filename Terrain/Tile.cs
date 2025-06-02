using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Terrain
{
    public class Tile
    {
        private static readonly Dictionary<char, Tile> _tileSet = new();

        public Char Ascii;
        public Color Color;
        public bool Walkable;
        public Tile(Char ascii, Color color, bool wakable)
        {
            Ascii = ascii;
            Color = color;
            Walkable = wakable;
            _tileSet.Add(ascii,this);
        }

        public static Tile CharToTile(Char chr)
        {
            return _tileSet[chr];
        }

    }
}
