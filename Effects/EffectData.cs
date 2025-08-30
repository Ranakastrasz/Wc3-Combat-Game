using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Effects
{

    public struct EffectData
    {
        public string EffectType; // e.g., "Damage", "Debuff"
        public string EffectSubtype; // E.g., "Dot", "Speed"
        public float Duration; // e.g., 5f seconds, 0 for instant
        public float Value; // e.g., 10f damage, 0.5f debuff strength

        EffectData(string effectType, string effectSubtype, float value, float duration)
        {
            EffectType = effectType;
            EffectSubtype = effectSubtype;
            Value = value;
            Duration = duration;
        }
    }
}

/* Effects we might need.
 *  Damage
 *  -Dot
 *  Heal
 *  -Hot
 *  debuff
 *  -speed
 *  -defense
 *  buff
 *  -speed
 *  -defense
 *  -shield
 *  charge
 */