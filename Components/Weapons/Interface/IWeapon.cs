using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;

namespace Wc3_Combat_Game.Components.Weapons.Interface
{
    public interface IWeapon
    {

        WeaponPrototype? GetPrototype();
        float GetAttackRange(); // For AI mostly.
        float GetAttackRangeSqr() => GetAttackRange() * GetAttackRange(); // For AI mostly.

        bool TryShootEntity(Unit unit, Entity target, IBoardContext context) { return false; }
        bool TryShootPoint(Unit unit, Vector2 target, IBoardContext context) { return false; }
        float GetCooldown();             // For display
        float GetTimeSinceLastShot(IDrawContext context); // For display

    }

}
