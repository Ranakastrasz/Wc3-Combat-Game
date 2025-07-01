using System.Numerics;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Prototype.Weapons;

namespace Wc3_Combat_Game.Components.Weapons.Interface
{
    public interface IWeapon
    {

        WeaponPrototype? GetPrototype();
        float AttackRange { get;}
        float AttackRangeSqr => AttackRange * AttackRange;

        bool TryShootEntity(Unit unit, Entity target, IBoardContext context) { return false; }
        bool TryShootPoint(Unit unit, Vector2 target, IBoardContext context) { return false; }

        float Cooldown { get; }             // For display

        float GetTimeSinceLastShot(IDrawContext context); // For display
        float GetTimeSinceLastShot(IBoardContext context); // For display

    }

}
