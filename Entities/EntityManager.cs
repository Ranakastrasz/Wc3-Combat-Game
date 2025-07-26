using Wc3_Combat_Game.Core;
using Wc3_Combat_Game.Core.Context;

namespace Wc3_Combat_Game.Entities
{
    public class EntityManager<T> where T : Entity
    {
        private readonly object _entityLock = new();

        private readonly List<T> _entities = new List<T>();
        public void UpdateAll(float deltaTime, IBoardContext context)
        {
            lock(_entityLock)
            {
                // Step 1: Initialize interactions for all active entities
                InitializeAllInteractions();

                // Step 2: Resolve pairwise interactions
                ResolveAllPairwiseInteractions(context);

                // Step 3: Perform individual entity updates based on resolved interactions
                foreach(var e in _entities.Where(e => e.IsAlive)) // Assuming IsAlive is on Entity
                {
                    e.Update(deltaTime, context);
                }
            }
        }

        private void InitializeAllInteractions()
        {
            // The EntityManager simply asks each entity to prepare for interaction calculations.
            // This might involve clearing accumulated forces, resetting state related to interactions, etc.
            foreach(var entity in _entities.Where(e => e.IsAlive))
            {
                entity.InitializeInteractionState();
            }
        }

        private void ResolveAllPairwiseInteractions(IBoardContext context)
        {
            // This is where the N^2 loop (or optimized spatial partitioning later) lives.
            // The EntityManager is responsible for identifying pairs that *might* interact.
            // The actual interaction logic is delegated to the entities.

            List<T> activeEntities = _entities.Where(e => e.IsAlive).ToList(); // Work with a snapshot

            for(int x = 0; x < activeEntities.Count; x++)
            {
                T entityA = activeEntities[x];

                for(int y = x + 1; y < activeEntities.Count; y++) // Avoid self-interaction and duplicate pairs
                {
                    T entityB = activeEntities[y];

                    // Bidirectional.
                    entityA.TryInteractWith(entityB, context);
                }
            }
        }
        public void Add(T entity)
        {
            //System.Diagnostics.Debug.WriteLine($"[EntityManager] Add: {entity} ({entity.GetType().Name}) at {Environment.StackTrace}");
            lock(_entityLock)
                _entities.Add(entity);
        }

        public void RemoveExpired(IBoardContext context)
        {
            //System.Diagnostics.Debug.WriteLine($"[EntityManager] RemoveExpired at {Environment.StackTrace}");
            //var before = _entities.Count;
            lock(_entityLock)
                for(int i = _entities.Count - 1; i >= 0; i--)
                {
                    var entity = _entities[i];
                    if(entity.IsExpired(context))
                    {
                        entity.Dispose();
                        _entities.RemoveAt(i);
                    }
                }
            //var after = _entities.Count;
            //System.Diagnostics.Debug.WriteLine($"[EntityManager] Removed {before - after} entities.");
        }
        public IReadOnlyList<T> Entities
        {
            get
            {
                lock(_entityLock)
                {
                    return _entities.ToList();
                }
            }
        }
        public void ForEach(Action<T> action)
        {
            //System.Diagnostics.Debug.WriteLine($"[EntityManager] ForEach at {Environment.StackTrace}");
            //IEnumerable<T> entities = _entities.ToList(); // To avoid modifying the collection while iterating

            lock(_entityLock)
                foreach(var entity in _entities)
                    action(entity);
        }

        public IEnumerable<T> GetEntitiesByTeam(Team team)
        {
            return _entities.Where(e => e.Team == team);
        }
        public IEnumerable<T> GetEntitiesByType(Type type)
        {
            return _entities.Where(e => e.GetType() == type);
        }

        public void Clear()
        {
            lock(_entityLock)
                _entities.Clear();
        }
    }
}
