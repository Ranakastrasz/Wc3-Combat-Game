using System;
using System.Collections.Generic;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Numerics;

namespace Wc3_Combat_Game.Core
{
    public static class GameConstants
    {
        public const int TICK_DURATION_MS = 16;              // Duration of one tick in milliseconds
        public const float FIXED_DELTA_TIME = TICK_DURATION_MS * 0.001f; // 0.016f

        public const float ENEMY_SPAWN_PADDING = 50f;

        public const float ENEMY_SPAWN_COOLDOWN = 1/8f;

        //#if DEBUG
        //        public static readonly Size DEBUG_PADDING = new(120, 120);
        //#else
        //    public static readonly Size DEBUG_PADDING = Size.Empty;
        //#endif
        private static readonly Size WINDOW_CLIENT_SIZE = new Size(1600, 800);
        //private static readonly Size WINDOW_CLIENT_SIZE = new Size(1600, 800) + DEBUG_PADDING;

        public static readonly Rectangle CLIENT_SIZE = new Rectangle(new(0, 0), WINDOW_CLIENT_SIZE);
        public static readonly RectangleF GAME_BOUNDS = new RectangleF(0, 0, 1024f, 1024f);
        public static readonly RectangleF CAMERA_BOUNDS = new RectangleF(0,0, 1000f, 800f);
        //public static readonly RectangleF CULL_BOUNDS = RectangleF.Inflate(GAME_BOUNDS, CULL_PADDING, CULL_PADDING);

        public static readonly RectangleF SPAWN_BOUNDS = RectangleF.Inflate(GAME_BOUNDS, ENEMY_SPAWN_PADDING, ENEMY_SPAWN_PADDING);
        public const float GAME_RESTART_DELAY = 3f;
    }
}
