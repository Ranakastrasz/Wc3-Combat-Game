using AStar;

using nkast.Aether.Physics2D.Dynamics;

using Wc3_Combat_Game.Entities;
using Wc3_Combat_Game.Terrain;

namespace Wc3_Combat_Game.Core.Context
{
    public interface IBoardContext : IContext
    {
        PathFinder PathFinder { get; }

        void AddProjectile(Projectile p);
        void AddUnit(Unit u);

        EntityManager<Entity> Entities { get; }
        World PhysicsWorld { get; }

        IReadOnlyList<Unit> GetFriendlyUnits(Team team);
        IReadOnlyList<Unit> GetEnemyUnits(Team team);

    }
}
