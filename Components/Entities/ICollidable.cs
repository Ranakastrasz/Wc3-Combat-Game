using System.Numerics;

using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Components.Entitys
{

    public interface ICollidable
    {
        public event EventHandler TerrainCollision;


        float CollisionRadius { get; }

        bool HasClearPathTo(Entity owner, Vector2 targetPosition, IBoardContext context) => HasClearPathTo(owner.Position, targetPosition, context);
        bool HasClearPathTo(Vector2 position, Vector2 targetPosition, IBoardContext context);
        void OnTerrainCollision(Entity owner, IBoardContext context);
        void Update(Entity owner, float deltaTime, IBoardContext context);
        bool CollidesAt(Entity owner, IBoardContext context);
        bool CollidesAt(Vector2 position, IBoardContext context);
    }
}
