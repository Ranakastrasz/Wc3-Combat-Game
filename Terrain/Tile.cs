﻿using Wc3_Combat_Game.Core.Context;
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
            float scale = context.Map.TileSize ;



            var location = GraphicsUtils.Scale(Position, scale);
            var size = new SizeF(scale, scale);
            var rect = new RectangleF(location, size);


            var camera = context.Camera.Viewport;

            if(GeometryUtils.Collides(rect, camera))
            {

                Brush brush = context.DrawCache.GetSolidBrush(GetColor);
                g.FillRectangle(brush, rect);

                if(context.DebugSettings.Get(DebugSetting.DrawMapCollisionTiles))
                {
                    if(!IsWalkable)
                    {
                        var pen = context.DrawCache.GetPen(Color.Red, 1);
                        g.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);
                    }
                }
            }
        }

    }
}
