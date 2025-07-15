using System.Numerics;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Components.Entitys
{
    public interface IMoveable
    {
        Vector2 Velocity { get; set; }
        void UpdateMovement(Entity owner, float deltaTime, IBoardContext context);
    }
}
