using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Components.Controllers.Interface
{
    public interface IUnitController
    {
        void DrawDebug(Graphics g, IDrawContext context, Unit unit);
        void Update(Unit unit, float deltaTime, IBoardContext context);
    }
}
