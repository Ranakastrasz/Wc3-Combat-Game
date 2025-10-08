namespace Wc3_Combat_Game.GameEngine.Terrain
{
    public class TileType
    {
        private static Dictionary<char, TileType> _byChar = new();
        private static Dictionary<string, TileType> _byName = new();

        static public readonly TileType Floor    = new TileType("Floor"   ,'.', Color.DarkGray, true);
        static public readonly TileType Wall     = new TileType("Wall"    ,'#', Color.Gray, false);
        static public readonly TileType Door     = new TileType("Door"    ,'_', Color.LightGray, true);
        static public readonly TileType Portal   = new TileType("Portal"  ,'P', Color.Purple, true);
        static public readonly TileType Fountain = new TileType("Fountain",'F', Color.Cyan, true);
        static public readonly TileType Shop     = new TileType("Shop"    ,'S', Color.Gray, false); // Pretend its a wall for now.




        public char Ascii; // Because maplookup. A real name might be better though.
                           // or, a double dictionary? I dunno.
        public Color Color; // Needs to draw itself I think.
        private bool Walkable;

        public bool IsWalkable => Walkable;

        public TileType(string name, char ascii, Color color, bool walkable)
        {
            Ascii = ascii;
            Color = color;
            Walkable = walkable;
            _byChar.Add(ascii, this);
            _byName.Add(name, this);
        }


        public static TileType FromChar(char c) => _byChar[c];
        public static TileType FromName(string name) => _byName[name];
    }
}
