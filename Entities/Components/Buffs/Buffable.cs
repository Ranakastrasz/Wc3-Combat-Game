using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Util;

using static Wc3_Combat_Game.Entities.Components.Interface.IBuffable;

namespace Wc3_Combat_Game.Entities.Components.Buffs;

// Attach to unit.
// This handles a short list of simplified, hardcoded debuffs
// Units will always query this type of object when getting modifiers for attributes.
// This may also handle some collision callback for the "Charge" effect, insofar as declaring that charge is active.


// Buff list.
// Speed, Slow, Shield, Vulnerable, Charge

// For now, Buffs just override the old one. no stacking, no smart-ness. I will probably have exactly
// one stat set for each buff for the time being anyway.
internal class Buffable: IBuffable
{
    float SpeedModifier = 1.0f;
    float SlowModifier = 1.0f;
    float DamageModifier = 1.0f;

    float SpeedExpires = float.NegativeInfinity;
    float SlowExpires = float.NegativeInfinity;
    float DamageModifierExpires = float.NegativeInfinity;
    float ChargeExpires = float.NegativeInfinity;
    float ShieldExpires = float.NegativeInfinity;

    public Buffable() { }

    public float GetFullSpeedModifier(IContext context)
    {
        return GetSpeedModifier(context) * GetSlowModifier(context);
    }

    private float GetSpeedModifier(IContext context)
    {
        if (TimeUtils.HasElapsed(context.CurrentTime, SpeedExpires, 0))
        {
            SpeedModifier = 1.0f;
        }
        return SpeedModifier;
    }

    private float GetSpeedDuration(IContext context)
    {
        if (TimeUtils.HasElapsed(context.CurrentTime, SpeedExpires, 0))
        {
            return 0.0f;
        }
        return SpeedExpires - context.CurrentTime;
    }
    private void SetSpeedBuff(float duration, float modifier, IContext context)
    {
        SpeedExpires = context.CurrentTime + duration;
        SpeedModifier = modifier;
    }

    private float GetSlowModifier(IContext context)
    {
        if(TimeUtils.HasElapsed(context.CurrentTime, SlowExpires, 0))
        {
            SlowModifier = 1.0f;
        }
        return SlowModifier;
    }
    private float GetSlowDuration(IContext context)
    {
        if (TimeUtils.HasElapsed(context.CurrentTime, SlowExpires, 0))
        {
            return 0.0f;
        }
        return SlowExpires - context.CurrentTime;
    }
    private void SetSlowBuff(float duration, float modifier, IContext context)
    {
        SlowExpires = context.CurrentTime + duration;
        SlowModifier = modifier;
    }

    private float GetDamageModifier(IContext context)
    {
        if (TimeUtils.HasElapsed(context.CurrentTime, DamageModifierExpires, 0))
        {
            DamageModifier = 1.0f;
        }
        return DamageModifier;
    }
    private float GetDamageDuration(IContext context)
    {
        if (TimeUtils.HasElapsed(context.CurrentTime, DamageModifierExpires, 0))
        {
            return 0.0f;
        }
        return DamageModifierExpires - context.CurrentTime;
    }
    private void SetDamageBuff(float duration, float modifier, IContext context)
    {
        DamageModifierExpires = context.CurrentTime + duration;
        DamageModifier = modifier;
    }

    private float GetChargeDuration(IContext context)
    {
        if (TimeUtils.HasElapsed(context.CurrentTime, ChargeExpires, 0))
        {
            return 0.0f;
        }
        return ChargeExpires - context.CurrentTime;
    }
    private void SetChargeBuff(float duration, IContext context)
    {
        ChargeExpires = context.CurrentTime + duration;
    }

    private float GetShieldDuration(IContext context)
    {
        if (TimeUtils.HasElapsed(context.CurrentTime, ShieldExpires, 0))
        {
            return 0.0f;
        }
        return ShieldExpires - context.CurrentTime;
    }

    private void SetShieldBuff(float duration, IContext context)
    {
        ShieldExpires = context.CurrentTime + duration;
    }


    public float GetBuffState(BuffType buff, IContext context)
    {
        return buff switch
        {
            BuffType.Speed => GetSpeedModifier(context),
            BuffType.Slow => GetSlowModifier(context),
            BuffType.Damage => GetDamageModifier(context),
            BuffType.Charge => GetChargeDuration(context) > 0 ? 1.0f : 0.0f,
            BuffType.Shield => GetShieldDuration(context) > 0 ? 1.0f : 0.0f,
            _ => 1.0f,
        };
    }

    public void ApplyBuff(BuffType buff, float duration, float modifier, IContext context)
    {
        switch (buff)
        {
            case BuffType.Speed:
                SetSpeedBuff(duration, modifier, context);
                break;
            case BuffType.Slow:
                SetSlowBuff(duration, modifier, context);
                break;
            case BuffType.Damage:
                SetDamageBuff(duration, modifier, context);
                break;
            case BuffType.Charge:
                SetChargeBuff(duration, context);
                break;
            case BuffType.Shield:
                SetShieldBuff(duration, context);
                break;
            default:
                throw new ArgumentException($"Unknown buff type: {buff}");
        }
    }
}

