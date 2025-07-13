using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.IO
{

    public interface IDrawContext
    {

        Map? Map { get; }
        float CurrentTime { get; }

        Camera? Camera { get; }
        EntityManager<Entity> Entities { get; }
        DebugSettings DebugSettings { get; }

        DrawCache DrawCache { get; }
    }
}
