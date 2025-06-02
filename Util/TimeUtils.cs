using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Util
{
    public static class TimeUtils
    {
        public static bool HasElapsed(float currentTime, float lastEventTime, float duration) =>
            currentTime >= lastEventTime + duration;
    }
}
