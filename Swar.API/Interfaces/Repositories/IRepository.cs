namespace Swar.API.Interfaces.Repositories
{
    public interface IRepository<K, T> where T : class
    {
        public Task<T> Add(T item);
        public Task<T> GetById(K key);
        public Task<T> Update(T item);
        public Task<T> Delete(K key);
        public Task<IEnumerable<T>> GetAll();
    }
}
