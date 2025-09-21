using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.PrototypeFactory;

namespace Wc3_Combat_Game.Data
{
    public static class PlayerBuilder
    {
        public static void BuildPlayer()
        {
            var player = new UnitPrototype("Player", 100f, 5f, 10f, 50f, Color.Blue, 1);
            //player = player.AddAbility(AbilityFactory.CreateInstantWeapon(5f, 1, 20, 0.5f, 1f));    
        }
    }
}