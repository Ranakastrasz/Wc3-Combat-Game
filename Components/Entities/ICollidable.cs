using System.Numerics;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Components.Entitys
{

    public interface ICollidable
    {
        float CollisionRadius { get; }
        bool IntersectsTerrain(Vector2 position, IBoardContext context);

        bool HasClearPathTo(Entity owner, Vector2 targetPosition, IBoardContext context) => HasClearPathTo(owner.Position, targetPosition, context);
        bool HasClearPathTo(Vector2 position, Vector2 targetPosition, IBoardContext context);
        void OnTerrainCollision(Entity owner, IBoardContext context);
    }
}
