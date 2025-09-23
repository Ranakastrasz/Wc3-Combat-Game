using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Core.Context;

using static Wc3_Combat_Game.Entities.Components.Buffs.Buffable;

namespace Wc3_Combat_Game.Entities.Components.Interface
{
    public interface IBuffable
    {
        public enum BuffType
        {
            Speed,
            Slow,
            Damage,
            Charge,
            Shield
        }

        public float GetBuffState(BuffType buff, IContext context);
        public void ApplyBuff(BuffType buff, float duration, float factor, IContext context);
    }
}