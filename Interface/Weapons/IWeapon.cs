using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype;

namespace Wc3_Combat_Game.Interface.Weapons
{
    public interface IWeapon
    {
        PrototypeWeapon? GetPrototype();
        float GetAttackRange(); // For AI mostly.
        float GetAttackRangeSqr() => GetAttackRange() * GetAttackRange(); // For AI mostly.

        bool TryShootEntity(Unit unit, IEntity target, BoardContext context) { return false; }
        bool TryShootPoint(Unit unit, Vector2 target, BoardContext context) { return false; }
        float GetCooldown();             // For display
        float GetTimeSinceLastShot(BoardContext context); // For display

    }

}
