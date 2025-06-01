using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game
{
    public enum TeamType
    {
        Ally,
        Enemy,
        Neutral
    }
    public static class AllegianceExtensions
    {
        public static bool IsHostileTo(this TeamType self, TeamType other)
        {
            if (self == TeamType.Neutral || other == TeamType.Neutral)
                return false;

            return self != other;
        }
        public static bool IsFriendlyTo(this TeamType self, TeamType other)
        {
            if (self == TeamType.Neutral || other == TeamType.Neutral)
                return false;

            return self == other;
        }
    }

}
