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

        public static void BuildPlayer()
        {

            AbilityPrototype weapon = AbilityFactory.CreateRangedWeapon(3f,0.5f,0.5f,600f,10f,0f,int.MaxValue,0.5f,2.5f,3,Color.Orange);

            UnitPrototype playerUnit = new("Player", 100f,  3f, 5f, 150f, Color.Green, 0);
            playerUnit = playerUnit.AddAbility(weapon);
            playerUnit = playerUnit with { MaxMana = 100, ManaRegen = 3f };

        }
    }
}