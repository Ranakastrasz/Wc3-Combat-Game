using System.Drawing.Drawing2D;
using System.Numerics;

using AssertUtils;

using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Util;

namespace Wc3_Combat_Game.Terrain
{
    public class TileMapRenderer: IDisposable
    {
        private Bitmap? _backgroundBitmap;
        private Graphics? _bitmapGraphics;
        private int _tileSize;
        private Tile[,]? _tileMap; // Your tilemap array
        private float _currentBuiltScale;

        private int Width { get; set; } // Width of the tilemap in tiles
        private int Height { get; set; } // Height of the tilemap in tiles

        // Constructor to set TileSize and IDrawContext
        public TileMapRenderer(int tileSize, Tile[,] tileMap)
        {
            _tileSize = tileSize;

            Width = tileMap.Length > 0 ? tileMap.GetLength(0) : 0;
            Height = tileMap.Length > 0 ? tileMap.GetLength(1) : 0;

            _tileMap = tileMap;
        }

        public void Draw(Graphics g, IDrawContext context)
        {
            AssertUtil.NotNull(context.Camera);
            float cameraScale = context.Camera.Zoom;
            Vector2 cameraPosition = context.Camera.Position;

            // Only rebuild the bitmap if the scale has changed significantly, or if it's null
            if(_backgroundBitmap == null || Math.Abs(_currentBuiltScale - cameraScale) > 0.001f) // Use a small tolerance for float comparison
            {
                BuildBitmap(context, cameraScale);
            }

            if(_backgroundBitmap != null)
            {
                GraphicsState oldState = g.Save();
                g.Transform = new Matrix(); // Reset Transform to Identity.

                Vector2 screenPoint = context.Camera.WorldPointToScreenPoint(new Vector2());
                g.InterpolationMode = InterpolationMode.NearestNeighbor; // Or Bilinear
                g.SmoothingMode = SmoothingMode.None; // Or AntiAlias for other elements if needed
                g.DrawImageUnscaled(_backgroundBitmap, screenPoint.ToPoint());

                g.Restore(oldState);
            }
        }

        private void BuildBitmap(IDrawContext context, float scale) // Now private and takes scale
        {
            // Dispose of existing bitmap and graphics object if they exist
            _bitmapGraphics?.Dispose();
            _backgroundBitmap?.Dispose();
            _bitmapGraphics = null;
            _backgroundBitmap = null;

            // Calculate the total size of the bitmap based on the new scale
            int scaledTileSize = (int)(_tileSize * scale);
            if(scaledTileSize == 0) scaledTileSize = 1; // Prevent zero size if scale is tiny

            int bitmapWidth = Width * scaledTileSize;
            int bitmapHeight = Height * scaledTileSize;

            if(bitmapWidth <= 0 || bitmapHeight <= 0)
            {
                // Handle cases where map might be empty or dimensions invalid after scaling
                return;
            }

            // Create a new Bitmap with the calculated size
            _backgroundBitmap = new Bitmap(bitmapWidth, bitmapHeight);
            // Optionally, specify pixel format for better performance/compatibility
            // _backgroundBitmap = new Bitmap(bitmapWidth, bitmapHeight, System.Drawing.Imaging.PixelFormat.Format32bppArgb);

            // Get a Graphics object from the bitmap. This is crucial!
            _bitmapGraphics = Graphics.FromImage(_backgroundBitmap);

            // Set high-quality rendering for the initial render to the bitmap
            _bitmapGraphics.CompositingQuality = CompositingQuality.HighQuality;
            _bitmapGraphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            _bitmapGraphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            _bitmapGraphics.SmoothingMode = SmoothingMode.HighQuality;

            // Loop through your tilemap and draw each tile onto the _bitmapGraphics
            for(int y = 0; y < Height; y++)
            {
                for(int x = 0; x < Width; x++)
                {
                    Tile tile = _tileMap[x, y];

                    // Directly draw the rectangle for the tile onto the bitmap graphics
                    var brush = context.DrawCache.GetSolidBrush(tile.GetColor);
                    Rectangle destRect = new Rectangle(x * scaledTileSize, y * scaledTileSize, scaledTileSize, scaledTileSize);
                    _bitmapGraphics.FillRectangle(brush, destRect);
                }
            }

            _currentBuiltScale = scale; // Store the scale the bitmap was built for
        }
        // Proper resource disposal
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if(disposing)
            {
                _bitmapGraphics?.Dispose();
                _backgroundBitmap?.Dispose();
                _bitmapGraphics = null;
                _backgroundBitmap = null;
                // If IDrawContext is managed by this class (unlikely, but for completeness)
                // _drawContext?.Dispose();
            }
        }
    }
}
