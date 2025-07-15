using Wc3_Combat_Game.Core;

namespace Wc3_Combat_Game.Entities
{
    public class EntityManager<T> where T : Entity
    {
        private readonly object _entityLock = new();

        private readonly List<T> _entities = new List<T>();
        public void UpdateAll(float deltaTime, IBoardContext context)
        {
            lock(_entityLock)
                foreach(var e in _entities) e.Update(deltaTime, context);
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
                _entities.RemoveAll(e => e.IsExpired(context));
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
