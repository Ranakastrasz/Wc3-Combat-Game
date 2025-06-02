using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Interface.Weapons;
using Wc3_Combat_Game.Prototype;

namespace Wc3_Combat_Game.Wave
{
    internal class Wave
    {
        public PrototypeUnit Unit { get; }
        public IWeapon Weapon { get; }
        public int Count { get; }
    
        internal Wave(PrototypeUnit unit, IWeapon weapon, int count)
        {
            Unit = unit;
            Weapon = weapon;
            Count = count;
        }
    }
}
