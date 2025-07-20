using System.Numerics;

using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities.Components.Interface
{

    public interface ICollidable
    {
        //public event EventHandler TerrainCollision;

        Vector2 Position { get; }

        float CollisionRadius { get; }
        Action<IBoardContext>? OnTerrainCollision { get; }

        // Include rectangle collision thing maybe.

        bool HasClearPathTo(Entity owner, Vector2 targetPosition, IBoardContext context) => HasClearPathTo(owner.Position, targetPosition, context);
        bool HasClearPathTo(Vector2 position, Vector2 targetPosition, IBoardContext context);
        void Update(Entity owner, float deltaTime, IBoardContext context);
        bool CollidesAt(Entity owner, IContext context);
        bool CollidesAt(Vector2 position, IContext context);
        void CheckCollision(Entity owner, Entity other, IBoardContext context);
    }
}
