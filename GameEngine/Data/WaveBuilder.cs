using Wc3_Combat_Game.Entities.Projectiles.Prototypes;
using Wc3_Combat_Game.Entities.Units.Abilities;
using Wc3_Combat_Game.Entities.Units.Buffs;
using Wc3_Combat_Game.Entities.Units.Prototypes;
using Wc3_Combat_Game.GameEngine.Actions;
using Wc3_Combat_Game.GameEngine.Waves;

namespace Wc3_Combat_Game.GameEngine.Data
{
    // For now, does basically all of it. Will split later or something.
    public static class WaveBuilder
    {
        public static void BuildWaves(List<Wave> _waves)
        {
            _waves.Add(new Wave("basic_enemy", 32));
            _waves.Add(new Wave("blitz_enemy", 32));
            _waves.Add(new Wave("blaster_enemy", 16));
            _waves.Add(new Wave("brute_enemy", 8));
            _waves.Add(new Wave("boss_enemy", 1));
            _waves.Add(new Wave("swarmer_enemy", 64));
            _waves.Add(new Wave("light_blaster_enemy", 32));
            _waves.Add(new Wave("elite_brute_enemy", 16));
            _waves.Add(new Wave("elite_blaster_enemy", 8));
            _waves.Add(new Wave("elite_boss_enemy", 1));

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
//Flesh Golem	    Boss    2000    200     1        Berserk + haste, health degenerates when low

//Skeleton Warrior	Swarmer 20	    10-15   128      Swarm type
//Skeleton Archer   Caster  25	    10-15   64       25 dmg spellbolt, elite 1-shots
//Skeleton Ork      Brute   100	    30-40   16       Tough brawler
//Skeleton Mage	    Caster  320	    25-30   8        Rare, 100 dmg bolt + snare, hitscan
//Lich Boss	        Boss    4000	200     1        Fan of 5 × 100 dmg bolts, snare, summon units
