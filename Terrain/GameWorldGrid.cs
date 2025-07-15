using AStar;

namespace Wc3_Combat_Game.Terrain
{
    public class GameWorldGrid
    {
        public readonly WorldGrid Grid;

        public int Width => Grid.Width;
        public int Height => Grid.Height;

        public GameWorldGrid(int width, int height)
        {
            Grid = new WorldGrid(height, width); // Note the swapped order
        }

        public GameWorldGrid(short[,] tileMap)
        {
            // Transpose the array for WorldGrid
            var transposed = new short[tileMap.GetLength(1), tileMap.GetLength(0)];
            for(int y = 0; y < tileMap.GetLength(1); y++)
                for(int x = 0; x < tileMap.GetLength(0); x++)
                    transposed[y, x] = tileMap[x, y];

            Grid = new WorldGrid(transposed);
        }

        public short this[int x, int y]
        {
            get => Grid[y, x];
            set => Grid[y, x] = value;
        }

        // Expose other needed members as pass-throughs
    }
}