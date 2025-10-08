using Wc3_Combat_Game.GameEngine.Terrain;

namespace Wc3_Combat_Game.Core.Context
{
    public interface IContext
    {
        float CurrentTime { get; }
        Map Map { get; }
    }
}