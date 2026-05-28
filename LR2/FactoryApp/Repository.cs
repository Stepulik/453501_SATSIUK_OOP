using System.Collections.Generic;

namespace FactoryLib
{
    public class Repository<T> where T : IEntity
    {
        private List<T> items = new List<T>();

        public void Add(T item) => items.Add(item);
        public IEnumerable<T> GetAll() => items;
        public T GetById(int id) => items.Find(item => item.Id == id);
    }
}