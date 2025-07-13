using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.IO;

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
            float cameraScale = context.Camera.Zoom;
            Vector2 cameraPosition = context.Camera.Position;

            // Only rebuild the bitmap if the scale has changed significantly, or if it's null
            if(_backgroundBitmap == null || Math.Abs(_currentBuiltScale - cameraScale) > 0.001f) // Use a small tolerance for float comparison
            {
                BuildBitmap(context, cameraScale);
            }

            if(_backgroundBitmap != null)
            {
                // *** IMPORTANT: Temporarily adjust the Graphics object's transform for drawing the bitmap ***

                // 1. Save the original transform of 'g'
                Matrix originalTransform = g.Transform;

                // 2. Create a new transform for drawing the bitmap.
                // The bitmap is already scaled to 'cameraScale'.
                // We only need to translate it to the correct position on the screen.
                // The camera's position is its *center* in world coordinates.
                // We need to translate the bitmap so its (0,0) world coordinate aligns
                // with the top-left of the camera's view on the screen.

                // Calculate the screen's top-left world coordinate based on camera position and zoom.
                // Assuming 'g.VisibleClipBounds.Width' and 'Height' give the actual pixel dimensions of the drawing area.
                // Note: g.VisibleClipBounds is affected by g.Transform.
                // It's safer to get the actual client size from the GameWindow_Paint method
                // and pass it down, or have DrawContext provide it.
                // For this example, let's assume screen dimensions are available (e.g., via context or passed in).
                // Let's assume you'll pass `ClientSize.Width` and `ClientSize.Height` from `GameWindow_Paint`
                // to `Map.Draw`, and then to `TileMapRenderer.Draw`.

                // For now, let's assume screen dimensions are available directly or through a property
                // on DrawContext, e.g., context.ScreenWidth, context.ScreenHeight.
                // If not, you'll need to pass them down from GameWindow_Paint.
                // For this example, I'll use placeholders for screen dimensions.
                float screenWidth = g.ClipBounds.Width; // This is problematic if g is already scaled
                float screenHeight = g.ClipBounds.Height; // This is problematic if g is already scaled

                // The most reliable way to get screen dimensions is to pass them from GameWindow_Paint.
                // So, let's adjust the signature of Draw:
                // public void Draw(Graphics g, IDrawContext context, int screenWidth, int screenHeight)
                // And in GameWindow_Paint: map.Draw(g, DrawContext, ClientSize.Width, ClientSize.Height);

                // Let's assume screenWidth and screenHeight are passed to this Draw method.
                // (I'll add them to the signature for clarity in the final code block).

                // Calculate the world coordinates of the top-left of the screen view.
                // CameraPosition is the center of the view.
                float worldViewTopLeftX = cameraPosition.X - (screenWidth / 2f / cameraScale);
                float worldViewTopLeftY = cameraPosition.Y - (screenHeight / 2f / cameraScale);

                // Create a translation matrix to position the bitmap.
                // The bitmap's (0,0) corresponds to world (0,0).
                // We need to shift the drawing origin so that the portion of the bitmap
                // starting at `worldViewTopLeftX, worldViewTopLeftY` aligns with the screen's (0,0).
                Matrix bitmapDrawTransform = new Matrix();
                bitmapDrawTransform.Translate(
                    -worldViewTopLeftX * cameraScale, // Translate by the negative of the world top-left, scaled
                    -worldViewTopLeftY * cameraScale
                );

                // Apply this temporary transform
                g.Transform = bitmapDrawTransform;

                // Draw the pre-scaled bitmap at its own origin (0,0).
                // Since the bitmap is already at the correct scale, and 'g' is only translated,
                // DrawImage will perform a fast pixel copy.
                g.DrawImage(_backgroundBitmap, 0, 0);

                // 3. Restore the original transform of 'g'
                // This is crucial so subsequent drawing (entities, UI) uses the camera's full transform.
                g.Transform = originalTransform;
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
                    var brush = context.DrawCache.GetOrCreateBrush(tile.GetColor);
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
