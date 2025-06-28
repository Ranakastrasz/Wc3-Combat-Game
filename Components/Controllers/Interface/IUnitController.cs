using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Components.Controllers.Interface
{
    public interface IUnitController
    {
        void DrawDebug(Graphics g, IDrawContext context, Unit unit);
        void Update(Unit unit, float deltaTime, IBoardContext context);
    }
}
