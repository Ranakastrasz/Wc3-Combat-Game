using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AssertUtils;

using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.Entities.Components.Prototype.PrototypeFactory;
using Wc3_Combat_Game.Waves;

namespace Wc3_Combat_Game.Data
{
    // For now, does basically all of it. Will split later or something.
    public static class WaveBuilder
    {
        public static void BuildWaves(List<Wave> _waves)
        {

            var rangedWeaponBase = new AbilityPrototype(
                    new ProjectileAction(new ProjectilePrototype("Ranged Weapon",2.5f, 225f, 4f, null, Color.DarkMagenta)), null,
                    0.5f,
                    150f,10f);

            var rangedWeaponSnare = new AbilityPrototype(
                    new ProjectileAction(new ProjectilePrototype("Snare Projectile",2.5f, 225, 16f,
                        new BuffAction(Entities.Components.Interface.IBuffable.BuffType.Slow,0.5f,1f), Color.Cyan)), null,
                    0.5f,
                    150f,5f);


            var unit = new UnitPrototype("Basic",15f, 2f, 4f, 50f, Color.Brown, 6);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(5f,1,20,0.0f,0.5f));
            _waves.Add(new Wave(unit, 32));
        
            unit = new UnitPrototype("Blitz",10f, 0.0f, 4f, 75f, Color.DarkGoldenrod, 3);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(10f, 1, 20, 0.0f, 0.5f));

            unit = new UnitPrototype("Blaster",30f, 0.0f, 5f, 40f, Color.Orange, 5);
            unit = unit.AddAbility(rangedWeaponBase.WithDamage(10f));
            _waves.Add(new Wave(unit, 16));
        
            unit = new UnitPrototype("Brute",80f, 2f, 10f, 50f, Color.Brown, 6);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(25f, 1, 20, 0.0f, 0.5f));
            _waves.Add(new Wave(unit, 8));

            unit = new UnitPrototype("Boss",400f, 0f, 15f, 100f, Color.DarkRed, 4);
            unit = unit.AddAbility(AbilityFactory.CreateInstantWeapon(90f, 2, 20, 0.0f, 0.5f));
            unit = unit.AddAbility(rangedWeaponSnare);
            _waves.Add(new Wave(unit, 1));
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
