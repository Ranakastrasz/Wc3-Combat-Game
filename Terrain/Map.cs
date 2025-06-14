using System.Numerics;
using AStar;
namespace Wc3_Combat_Game.Terrain
{
    public class Map
    {

        

        public Tile[,] TileMap;
        public WorldGrid PathfinderGrid;
        public float TileSize { get; set; }
        public int Width;
        public int Height;

        Map(Tile[,] tileMap, float tileSize)
        {
            TileMap = tileMap;
            TileSize = tileSize;
            short[,] pathfinderGrid = new short[tileMap.GetLength(0), tileMap.GetLength(1)];
            this.PathfinderGrid = new WorldGrid(pathfinderGrid);
            UpdatePathing();
        }

        public static Map ParseMap(string[] mapString, float tileSize = 1f)
        {

            Tile[,] tileMap = new Tile[mapString[0].Length, mapString.Length];
            for (int y = 0; y < mapString.Length; y++)
            {
                for (int x = 0; x < mapString[y].Length; x++)
                {
                    char c = mapString[y][x];
                    tileMap[x, y] = new(TileType.FromChar(c),new(x,y));
                }
            }
            Map newMap = new(tileMap, tileSize)
            {
                Width = mapString[0].Length,
                Height = mapString.Length
            };
            return newMap;
        }

        internal List<Point> GetTilesMatching(char chr)
        {
            List<Point> matchingTiles = new();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    if (TileMap[x, y].GetChar == 'P')
                    {
                        matchingTiles.Add(new Point(x, y));
                    }
                }
            }
            return matchingTiles;
        }

        public Point ToGrid(Vector2 worldPosition)
        {
            int x = (int)Math.Floor(worldPosition.X / TileSize);
            int y = (int)Math.Floor(worldPosition.Y / TileSize);

            return new(x,y);
        }

        public Vector2 FromGrid(int x, int y)
        {
            return new Vector2((x + 0.5f) * TileSize, (y + 0.5f) * TileSize);
        }

        public Vector2 FromGrid(Point index)
        {
            return FromGrid(index.X, index.Y);
        }

        public Vector2 GetTileCenter(Vector2 worldPosition)
        {
            int x = (int)Math.Floor(worldPosition.X / TileSize);
            int y = (int)Math.Floor(worldPosition.Y / TileSize);
            return FromGrid(x, y);
        }

        public bool CollidesAt(Vector2 pos, float radius)
        {// Needs to be in GeometryUtil.. Or Map. Pass map, radius, Position.
            Map map = this;

            int minTileX = (int)Math.Floor((pos.X - radius) / TileSize);
            int maxTileX = (int)Math.Floor((pos.X + radius) / TileSize);
            int minTileY = (int)Math.Floor((pos.Y - radius) / TileSize);
            int maxTileY = (int)Math.Floor((pos.Y + radius) / TileSize);

            for (int y = minTileY; y <= maxTileY; y++)
            {
                for (int x = minTileX; x <= maxTileX; x++)
                {
                    if (x < 0 || y < 0 || x >= map.Width || y >= map.Height)
                        return true; // Treat out-of-bounds as solid

                    Tile tile = map[x, y];
                    if (!tile.IsWalkable)
                    {
                        Vector2 tileMin = new(x * TileSize, y * TileSize);
                        Vector2 tileMax = tileMin + new Vector2(TileSize);
                        Vector2 closestPoint = Vector2.Clamp(pos, tileMin, tileMax);
                        float distSq = Vector2.DistanceSquared(pos, closestPoint);
                        if (distSq < radius * radius)
                            return true;
                    }
                }
            }
            return false;
        }

        public bool HasLineOfSight(Point myTile, Point targetTile)
        {
            return HasLineOfSight(FromGrid(myTile), FromGrid(targetTile));
        }
        public bool HasLineOfSight(Vector2 startWorld, Vector2 targetWorld)
        {
            Point startGrid = ToGrid(startWorld);
            Point targetGrid = ToGrid(targetWorld);

            int x0 = startGrid.X;
            int y0 = startGrid.Y;
            int x1 = targetGrid.X;
            int y1 = targetGrid.Y;

            int dx = Math.Abs(x1 - x0);
            int dy = Math.Abs(y1 - y0);
            int sx = x0 < x1 ? 1 : -1;
            int sy = y0 < y1 ? 1 : -1;
            int err = dx - dy;

            while (true)
            {
                if (x0 < 0 || x0 >= Width || y0 < 0 || y0 >= Height)
                    return false; // Out of bounds blocks LOS (you can adjust this)

                Tile currentTile = this[x0, y0];
                if (currentTile != null && !currentTile.IsWalkable && !(x0 == startGrid.X && y0 == startGrid.Y) && !(x0 == targetGrid.X && y0 == targetGrid.Y))
                {
                    return false; // Non-walkable tile blocks LOS (excluding start and end)
                }

                if (x0 == x1 && y0 == y1)
                    break;

                int e2 = 2 * err;
                if (e2 > -dy)
                {
                    err -= dy;
                    x0 += sx;
                }
                if (e2 < dx)
                {
                    err += dx;
                    y0 += sy;
                }
            }

            return true;
        }

        public List<Point> GetAdjacentTiles(int x, int y)
        {
            List<Point> adjacent = new List<Point>();
            int[] dx = [ 0, 1, 0,-1];
            int[] dy = [-1, 0, 1, 0];

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < Width && ny >= 0 && ny < Height)
                {
                    adjacent.Add(new Point(nx,ny));
                }
            }
            return adjacent;
        }

        public List<Point> GetAdjacentTiles(Point index)
        {
            return GetAdjacentTiles(index.X, index.Y);
        }

        internal List<Tile> GetWalkableNeighbors(Tile p)
        {
            //GetAdjacentTiles(p.Position)
            return new List<Tile>();
        }

        public void UpdatePathing()
        {
            for (int y = 0; y < TileMap.GetLength(1); y++)
            {
                for (int x = 0; x < TileMap.GetLength(0); x++)
                {
                    Tile tile = TileMap[x, y];
                    PathfinderGrid[x, y] = tile.IsWalkable ? (short)1 : (short)0;
                }
            }
        }
        public Tile this[int x, int y]
        {
            get => TileMap[x, y];
            set => TileMap[x, y] = value;
        }
        public Tile this[Point index]
        {
            get => TileMap[index.X, index.Y];
            set => TileMap[index.X, index.X] = value;
        }
    }
}
