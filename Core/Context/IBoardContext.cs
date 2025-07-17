using AStar;

using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Core.Context
{
    public interface IBoardContext : IContext
    {
        Map Map { get; }

        PathFinder PathFinder { get; }

        void AddProjectile(Projectile p);
        void AddUnit(Unit u);

        EntityManager<Entity> Entities { get; }

        IReadOnlyList<Unit> GetFriendlyUnits(Team team);
        IReadOnlyList<Unit> GetEnemyUnits(Team team);

    }
}
