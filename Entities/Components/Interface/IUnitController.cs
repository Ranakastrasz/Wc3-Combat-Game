using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.EntityTypes;

namespace Wc3_Combat_Game.Entities.Components.Interface
{
    public interface IUnitController
    {
        void DrawDebug(Graphics g, IDrawContext context, Unit unit);
        void Update(Unit unit, float deltaTime, IBoardContext context);
    }
}
