using System.Numerics;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Prototype.Abilities;

namespace Wc3_Combat_Game.Entities.Components.Interface
{
    public interface IAbility
    {

        AbilityPrototype? GetPrototype();

        bool TryTargetEntity(Unit unit, Entity target, IBoardContext context) { return false; }
        bool TryTargetPoint(Unit unit, Vector2 target, IBoardContext context) { return false; }

        bool CanUse(Unit unit, IContext context) { return false; }

        float Cooldown { get; }             // For display

        float GetTimeSinceLastUse(IContext context); // For display
        bool OnCooldown(float currentTime);
    }

}
