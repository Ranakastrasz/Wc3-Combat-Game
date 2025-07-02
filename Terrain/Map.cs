using System.Numerics;
using AStar;
using Wc3_Combat_Game.Util;
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

        /// <summary>
        /// Checks if a circle at a given world position collides with any non-walkable tile.
        /// This method iterates through the tiles that the circle's bounding box might overlap.
        /// </summary>
        /// <param name="pos">The center world position of the circle.</param>
        /// <param name="radius">The radius of the circle.</param>
        /// <returns>True if the circle collides with any non-walkable tile, false otherwise.</returns>
        public bool CollidesAt(Vector2 pos, float radius)
        {
            // Calculate the tile range that the circle at 'pos' with 'radius' might overlap.
            int minTileX = (int)Math.Floor((pos.X - radius) / TileSize);
            int maxTileX = (int)Math.Ceiling((pos.X + radius) / TileSize) - 1; // Use Ceil and -1 to get inclusive max tile
            int minTileY = (int)Math.Floor((pos.Y - radius) / TileSize);
            int maxTileY = (int)Math.Ceiling((pos.Y + radius) / TileSize) - 1; // Use Ceil and -1 to get inclusive max tile

            // Clamp tile coordinates to map bounds
            minTileX = Math.Max(0, minTileX);
            maxTileX = Math.Min(Width - 1, maxTileX);
            minTileY = Math.Max(0, minTileY);
            maxTileY = Math.Min(Height - 1, maxTileY);

            // Iterate through all potential tiles within the circle's bounding box.
            for(int y = minTileY; y <= maxTileY; y++)
            {
                for(int x = minTileX; x <= maxTileX; x++)
                {
                    // If the tile is out of map bounds, treat it as a collision.
                    // This check is technically redundant if min/max are clamped, but good as a fallback.
                    if(x < 0 || y < 0 || x >= Width || y >= Height)
                        return true;

                    Tile tile = this[x, y];
                    // If the tile is not walkable, check for actual collision with the circle.
                    if(!tile.IsWalkable)
                    {
                        // Calculate the min and max world coordinates of the current tile.
                        Vector2 tileMin = new(x * TileSize, y * TileSize);
                        Vector2 tileMax = tileMin + new Vector2(TileSize);

                        // Use the helper to check circle-rectangle collision for this specific tile.
                        if(GeometryUtils.CollidesCircleWithRectangle(pos, radius, tileMin, tileMax))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasLineOfSight(Point myTile, Point targetTile)
        {
            return HasLineOfSight(FromGrid(myTile), FromGrid(targetTile));
        }
        public bool HasLineOfSight(Vector2 startWorld, Vector2 targetWorld, float radius = 0f)
        {
            // Calculate the bounding box of the "capsule" (line segment + radius) in world coordinates.
            float minX = Math.Min(startWorld.X, targetWorld.X) - radius;
            float maxX = Math.Max(startWorld.X, targetWorld.X) + radius;
            float minY = Math.Min(startWorld.Y, targetWorld.Y) - radius;
            float maxY = Math.Max(startWorld.Y, targetWorld.Y) + radius;

            // Convert the world bounding box to tile coordinates.
            int minTileX = (int)Math.Floor(minX / TileSize);
            int maxTileX = (int)Math.Ceiling(maxX / TileSize) - 1;
            int minTileY = (int)Math.Floor(minY / TileSize);
            int maxTileY = (int)Math.Ceiling(maxY / TileSize) - 1;

            // Clamp tile coordinates to map bounds.
            minTileX = Math.Max(0, minTileX);
            maxTileX = Math.Min(Width - 1, maxTileX);
            minTileY = Math.Max(0, minTileY);
            maxTileY = Math.Min(Height - 1, maxTileY);

            // Iterate through all tiles within the expanded bounding box.
            for(int y = minTileY; y <= maxTileY; y++)
            {
                for(int x = minTileX; x <= maxTileX; x++)
                {
                    Tile tile = this[x, y];

                    // If the tile is non-walkable, check for collision with the capsule.
                    // We also exclude the start and end tiles from blocking, as the object might be on them.
                    // However, if the *radius* of the object at start/end overlaps an obstacle, it IS a collision.
                    // The logic here is: if the tile is an obstacle AND it's not the exact start/end tile
                    // (to allow seeing *from* a tile a unit is on), then check for collision.
                    // A more robust approach might check if start/end points are *valid* positions first.
                    // For simplicity, we'll allow the start/end *tile* to be an obstacle if the object is there.
                    // But if the swept path *through* that tile or any other tile collides, LOS is blocked.

                    // To simplify and be more accurate for "swept volume":
                    // If the tile is an obstacle, check if the capsule intersects it.
                    if(!tile.IsWalkable)
                    {
                        Vector2 tileMin = new(x * TileSize, y * TileSize);
                        Vector2 tileMax = tileMin + new Vector2(TileSize);

                        // Check if the start or end "hemisphere" (circle) collides with this obstacle tile.
                        if(GeometryUtils.CollidesCircleWithRectangle(startWorld, radius, tileMin, tileMax) ||
                            GeometryUtils.CollidesCircleWithRectangle(targetWorld, radius, tileMin, tileMax))
                        {
                            return false; // Start or end point's radius overlaps an obstacle
                        }

                        // Check if the line segment (inflated by radius) intersects this obstacle tile.
                        // This is equivalent to checking if the line segment intersects the tile's AABB
                        // expanded by the radius.
                        Vector2 inflatedTileMin = tileMin - new Vector2(radius);
                        Vector2 inflatedTileMax = tileMax + new Vector2(radius);

                        if(GeometryUtils.LineSegmentIntersectsAABB(startWorld, targetWorld, inflatedTileMin, inflatedTileMax))
                        {
                            return false; // The swept body of the capsule collides with an obstacle
                        }
                    }
                }
            }

            return true; // No collision found along the path
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
