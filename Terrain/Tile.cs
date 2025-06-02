using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Terrain
{
    public class Tile
    {
        private static readonly Dictionary<char, Tile> _tileRegistry = new();

        static public readonly Tile Floor = new Tile('.', Color.DarkGray, true);
        static public readonly Tile Wall = new Tile('#', Color.Gray, false);
        static public readonly Tile Portal = new Tile('P', Color.Purple, true);
        static public readonly Tile Fountain = new Tile('F', Color.Blue, true);
        static public readonly Tile Shop = new Tile('S', Color.Cyan, false);

        public Char Ascii; // Because maplookup. A real name might be better though.
                            // or, a double dictionary? I dunno.
        public Color Color; // Needs to draw itself I think.
        private bool Walkable;

        public bool IsWalkable => Walkable;

        public Tile(Char ascii, Color color, bool walkable)
        {
            Ascii = ascii;
            Color = color;
            Walkable = walkable;
            _tileRegistry.Add(ascii,this);
        }

        public static Tile CharToTile(Char chr)
        {
            return _tileRegistry[chr];
        }

    }
}
