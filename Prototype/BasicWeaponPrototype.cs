using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Effects;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Prototype
{
    internal class BasicWeaponPrototype : ICloneable
    {
        public Effect? CastEffect { get; set; }
        public float Cooldown { get; set; }
        public float CastRange { get; set; }

        public BasicWeaponPrototype(Effect castEffect, float cooldown, float castRange)
        {
            CastEffect = castEffect;
            Cooldown = cooldown;
            CastRange = castRange;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}
