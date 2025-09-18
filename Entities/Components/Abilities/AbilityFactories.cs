using Wc3_Combat_Game.Actions;
using Wc3_Combat_Game.Entities.Components.Interface;
using Wc3_Combat_Game.Entities.Components.Prototype;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;

namespace Wc3_Combat_Game.Entities.Components.Abilities
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
        public static AbilityPrototype CreateMeleeWeapon(float damage)
        {
            // Add more parameters like attack speed, range if needed
            return new AbilityPrototype(null, null, 1f, 20f).WithDamage(damage);
        }

        public static AbilityPrototype CreateRangedWeapon(float damage)
        {
            ProjectileAction projectile = new ProjectileAction(new ProjectilePrototype("Ranged Weapon", 2.5f, 225f, 4f, null, Color.DarkMagenta));
            return new AbilityPrototype(projectile, null, 0.5f, 150f, 10f).WithDamage(damage);
        }

        public static AbilityPrototype CreateSnareWeapon()
        {
            ProjectileAction projectile = new ProjectileAction(new ProjectilePrototype("Snare Projectile", 2.5f, 225, 16f,
            new BuffAction(IBuffable.BuffType.Slow, 1f, 0.5f), Color.Cyan));
            return new AbilityPrototype(projectile, null, 0.5f, 150f, 5f);
        }
    }
}
