using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Wc3_Combat_Game.Core;

namespace Wc3_Combat_Game.Entities
{
    public class EntityManager<T> where T : IEntity
    {
        private readonly List<T> _entities = new List<T>();
        public void UpdateAll(float deltaTime, BoardContext context)
            { foreach (var e in _entities) e.Update(deltaTime,context); }
        public void Add(T entity) { _entities.Add(entity); }
        public void RemoveExpired(BoardContext context)
        {
            _entities.RemoveAll(e => e.IsExpired(context));
        }
        public IReadOnlyList<T> Entities => _entities;
        public void ForEach(Action<T> action)
        {
            foreach (var entity in _entities)
                action(entity);
        }
    }
}
