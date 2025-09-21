using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;

namespace Wc3_Combat_Game.Entities.Components.Prototype.PrototypeFactory
{
    // Factory for weapon varients, I think.
    //
    // Melee attack
    // Ranged attack
    // Ranged Mana costing attack.
    // Ranged hitscan debuff
    // Self buff
    // Self projectile charge thinggy.
    // Summon
    //
    // Really though, probably just make for each ability archetype i end up with.
    // So, include Fan, Extra payload attachment, AOE, etc.
    // Or, its, create ability, attach projectile, attach effect to projectile.


    public class AbilityFactory
    {


        public static AbilityPrototype CreateInstantWeapon(float damage, float cooldown, float range, float? recoilFactor = null, float recoilDuration = 0f)
        {

            AbilityPrototype prototype = new AbilityPrototype(null, null, 1f, 20f).WithDamage(damage);


            if (recoilFactor != null && recoilDuration != 0f)
            {
                prototype = prototype with      {
                    CasterEffect = new BuffAction(IBuffable.BuffType.Slow, recoilFactor.Value, recoilDuration)
                };
            }

            return prototype;
        }

        //public static AbilityPrototype CreateRangedWeapon(float damage)
        //{
        //    ProjectileAction projectile = new ProjectileAction(new ProjectilePrototype("Ranged Weapon", 2.5f, 225f, /4f, /null, Color.DarkMagenta));
        //    return new AbilityPrototype(projectile, null, 0.5f, 150f, 10f).WithDamage(damage);
        //}
        //
        //public static AbilityPrototype CreateSnareWeapon()
        //{
        //    ProjectileAction projectile = new ProjectileAction(new ProjectilePrototype("Snare Projectile", 2.5f, /225, /16f,
        //    new BuffAction(IBuffable.BuffType.Slow, 0.5f, 1f), Color.Cyan));
        //    return new AbilityPrototype(projectile, null, 0.5f, 150f, 5f);
        //}
    }
}
