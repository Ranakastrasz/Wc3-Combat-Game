using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Core.Context
{

    public interface IDrawContext: IContext
    {

        Map Map { get; }

        Camera Camera { get; }
        EntityManager<Entity> Entities { get; }
        DebugSettings DebugSettings { get; }

        DrawCache DrawCache { get; }
    }
}
