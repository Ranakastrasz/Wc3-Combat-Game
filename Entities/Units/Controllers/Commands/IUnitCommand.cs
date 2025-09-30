using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Wc3_Combat_Game.Core.Context;
using Wc3_Combat_Game.Entities.Components.Interface;

namespace Wc3_Combat_Game.Entities.Units.Controllers.Commands
{
    public interface IUnitCommand
    {
        // The command is executed against the Unit
        void Execute(IBoardContext context);
        void Execute(Unit unit, IBoardContext context);
        void Execute(Vector2 position, IBoardContext context);
    }
}
