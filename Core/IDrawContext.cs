using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Core
{
    public interface IDrawContext
    {

        Map? Map { get; }
        float CurrentTime { get; }

        EntityManager<Entities.Entity> Entities { get; }
    }
}
