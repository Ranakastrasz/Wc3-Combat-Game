using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Entities;

namespace Wc3_Combat_Game.Interface.Controllers
{
    public interface IUnitController
    {
        void Update(Unit unit, float deltaTime, BoardContext context);
    }
}
