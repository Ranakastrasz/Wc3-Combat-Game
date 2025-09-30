using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.VisualStyles;

using AssertUtils;

using nkast.Aether.Physics2D.Controllers;

using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities.Components.Controllers;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;
using Wc3_Combat_Game.Entities.Components.Prototype.PrototypeFactory;
using Wc3_Combat_Game.Entities.EntityTypes;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Data
{
    public static class PlayerBuilder
    {

        private static AbilityPrototype BuildManabolt()
        {
            AbilityPrototype weapon = AbilityFactory.CreateRangedWeapon(3f,0.5f,0.5f,600f,10f,0f,int.MaxValue,0.2f,2.5f,3,Color.Orange);
            return weapon;
        }

        private static AbilityPrototype BuildManabomb()
        {
            AbilityPrototype weapon = AbilityFactory.CreateRangedWeapon(20f,0.5f,1f,450f,30f,32f,int.MaxValue,1f,5f,int.MaxValue,Color.Orange);
            return weapon;
        }

        private static AbilityPrototype BuildSprint()
        { 
            AbilityPrototype sprint = new AbilityPrototype(
                null,
                new BuffAction(Entities.Components.Interface.IBuffable.BuffType.Speed,3f,0.25f),
                3f,
                0f,
                15f);
            return sprint;
        }

        public static UnitPrototype BuildPlayer()
        {

            AbilityPrototype manabolt = BuildManabolt();
            AbilityPrototype manabomb = BuildManabomb();
            AbilityPrototype sprint = BuildSprint();


            // Temperary unsafe crap til I have proper builder.

            //ProjectileAction targetEffect = weapon.TargetEffect as ProjectileAction;
            //targetEffect = targetEffect with { ProjectileCount = 3, FullSpreadAngleDeg = 15f };
            //weapon = weapon with { TargetEffect = targetEffect };

            UnitPrototype playerUnit = new("Player", 100f,  3f, 5f, 150f, Color.Green, 0);
            playerUnit = playerUnit.AddAbility(manabolt);
            playerUnit = playerUnit.AddAbility(manabomb);
            playerUnit = playerUnit.AddAbility(sprint);
            
            playerUnit = playerUnit with { MaxMana = 100, ManaRegen = 3f };

            return playerUnit;
        }
    }
}