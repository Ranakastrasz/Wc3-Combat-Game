using AssertUtils;
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

        public void Draw(Graphics g, IDrawContext context)
        {
            AssertUtil.NotNull(context.Map);
            AssertUtil.NotNull(context.Camera);
            float scale = context.Map.TileSize ;



            var location = GraphicsUtils.Scale(Position, scale);
            var size = new SizeF(scale, scale);
            var rect = new RectangleF(location, size);


            var camera = context.Camera.Viewport;

            if(GeometryUtils.Collides(rect, camera))
            {

                Brush brush = context.DrawCache.GetOrCreateBrush(GetColor);
                g.FillRectangle(brush, rect);
            }
        }
    }
}
