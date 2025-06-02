using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Core
{
    public interface IDrawContext
    {

        Tile[,] TileGrid { get; }
        float CurrentTime { get; }
        Unit PlayerUnit { get; }


    }
}
