using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Wc3_Combat_Game.Entities
{
    class EntityManager<T> where T : IEntity
    {
        private readonly List<T> _entities = new List<T>();
        public void UpdateAll(float deltaTime, float currentTime)
            { foreach (var e in _entities) e.Update(deltaTime,currentTime); }
        public void Add(T entity) { _entities.Add(entity); }
        public void RemoveExpired(double currentTime)
        {
            _entities.RemoveAll(e => e.IsExpired(currentTime));
        }
        public IReadOnlyList<T> Entities => _entities;
        public void ForEach(Action<T> action)
        {
            foreach (var entity in _entities)
                action(entity);
        }
    }
}
