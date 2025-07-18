﻿namespace Wc3_Combat_Game.Core
{
    public enum Team
    {
        Ally,
        Enemy, // May need something else someday, but this is valid for now.
        Neutral
    }
    public static class TeamExtensions
    {
        public static bool IsHostileTo(this Team self, Team other)
        {
            if(self == Team.Neutral || other == Team.Neutral)
                return false;

            return self != other;
        }
        public static bool IsFriendlyTo(this Team self, Team other)
        {
            if(self == Team.Neutral || other == Team.Neutral)
                return false;

            return self == other;
        }

    }

}
