using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.IO;

namespace Wc3_Combat_Game.Core.Context
{

    public interface IDrawContext: IContext
    {


        Camera Camera { get; }
        EntityManager<Entity> Entities { get; }
        DebugSettings DebugSettings { get; }

        DrawCache DrawCache { get; }
    }
}
