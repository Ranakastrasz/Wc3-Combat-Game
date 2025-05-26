using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game
{
    public static class GameConstants
    {
        public const int TickDurationMs = 16;              // Duration of one tick in milliseconds
        public const float FixedDeltaTime = TickDurationMs * 0.001f; // 0.016f
    }
}
