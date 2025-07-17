using Wc3_Combat_Game.Entities.Components.Interface;

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
        public IAbility CreateMeleeAttack()
        {
            throw new NotImplementedException();
        }
    }

}
