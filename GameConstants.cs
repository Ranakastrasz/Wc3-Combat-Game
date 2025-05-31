using System;
using System.Collections.Generic;
using System.Drawing.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace Wc3_Combat_Game
{
    public static class GameConstants
    {
        public const int TICK_DURATION_MS = 16;              // Duration of one tick in milliseconds
        public const float FIXED_DELTA_TIME = TICK_DURATION_MS * 0.001f; // 0.016f


        public const float PLAYER_SPEED = 300f; // Hardcoded for now.
        public static readonly SizeF PLAYER_SIZE = new(50, 50);
        public static readonly Brush PLAYER_COLOR = Brushes.Green;

        public static readonly float PLAYER_COOLDOWN = 1.0f;

        public const float PROJECTILE_SPEED = 1200f;
        public static readonly SizeF PROJECTILE_SIZE = new(10, 10);
        public static readonly Brush PROJECTILE_COLOR = Brushes.Blue;

        public const float PROJECTILE_LIFESPAN = 0.25f;


        public const float ENEMY_SPEED = 200f;
        public static readonly SizeF ENEMY_SIZE = new(25, 25);
        public static readonly Brush ENEMY_COLOR = Brushes.Red;

        public const float ENEMY_SPAWN_TIMER = 0.25f;
        public const float ENEMY_SPAWN_PADDING = 50f;


        public const float ENEMY_SPAWN_COOLDOWN = 0.5f;
        public const int ENEMY_WAVE_SIZE = 32;

        //public const float CULL_PADDING = 100f;

#if DEBUG
        public static readonly Size DEBUG_PADDING = new(120, 120);
#else
    public static readonly Size DEBUG_PADDING = Size.Empty;
#endif
        private static readonly Size WINDOW_CLIENT_SIZE = new Size(1200, 800) + DEBUG_PADDING;

        public static readonly Rectangle CLIENT_SIZE = new Rectangle(new(0, 0), WINDOW_CLIENT_SIZE);
        public static readonly RectangleF GAME_BOUNDS = RectangleF.Inflate(CLIENT_SIZE, -DEBUG_PADDING.Width, -DEBUG_PADDING.Height); // or dynamically from Form.Size
        //public static readonly RectangleF PLAYER_BOUNDS = RectangleF.Inflate(GAME_BOUNDS, -PLAYER_SIZE.Width / 2f, -PLAYER_SIZE.Height / 2f);
        //public static readonly RectangleF CULL_BOUNDS = RectangleF.Inflate(GAME_BOUNDS, CULL_PADDING, CULL_PADDING);

        public static readonly RectangleF SPAWN_BOUNDS = RectangleF.Inflate(GAME_BOUNDS, ENEMY_SPAWN_PADDING, ENEMY_SPAWN_PADDING);

        public const float GAME_RESTART_DELAY = 3f;
    }
}
