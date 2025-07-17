using System.Numerics;

using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities.Components.Interface
{
    public interface IMoveable
    {
        Vector2 Velocity { get; set; }
        void Update(Entity owner, float deltaTime, IBoardContext context);
    }
}
