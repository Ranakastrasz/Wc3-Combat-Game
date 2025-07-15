using Wc3_Combat_Game.Prototype;

namespace Wc3_Combat_Game.Waves
{
    public struct Wave
    {
        public UnitPrototype Unit { get; }
        public int CountToSpawn { get; }

        public Wave(UnitPrototype unit, int toSpawn)
        {
            Unit = unit;
            CountToSpawn = toSpawn;
        }
    }
}




//
//public static UnitPrototype ENEMY_SWARMER_LIGHT   = new(   5f, 2f  ,  20f, 125f, Brushes.Red, UnitPrototype.DrawShape.Circle);
//public static UnitPrototype ENEMY_SWARMER         = new(  10f, 0.1f,  30f, 150f, Brushes.Red, UnitPrototype.DrawShape.Circle);
//public static UnitPrototype ENEMY_CASTER          = new(  30f, 0.1f,  30f, 100f, Brushes.Orange, UnitPrototype.DrawShape.Square);
//public static UnitPrototype ENEMY_BRUTE           = new(  80f, 2f  ,  50f, 125f, Brushes.Red, UnitPrototype.DrawShape.Square);
//public static UnitPrototype ENEMY_BRUTE_BOSS      = new(1000f, 0f  , 100f, 125f, Brushes.DarkRed, UnitPrototype.DrawShape.Square);

//Unit              Type    HP      Dmg     Qty      Notes
//Zombie	        Swarmer  5	    5-10    64       1 regen, trivial
//Ghoul	            Swarmer 10	    10-20   64       Common
//Acolyte	        Caster  30	    10-15   32       10 dmg spellbolt
//Abomination	    Brute   80	    25-30   16       2 regen, ~25 dmg, elite 1-shots
//Meat Golem	    Boss    2000    200     1        Berserk + haste, health degenerates when low

//Skeleton Warrior	Swarmer 20	    10-15   128      Swarm type
//Skeleton Archer   Caster  25	    10-15   64       25 dmg spellbolt, elite 1-shots
//Skeleton Ork      Brute   100	    30-40   16       Tough brawler
//Skeleton Mage	    Caster  320	    25-30   8        Rare, 100 dmg bolt + snare, hitscan
//Lich Boss	        Boss    4000	200     1        Fan of 5 × 100 dmg bolts, snare, summon units
