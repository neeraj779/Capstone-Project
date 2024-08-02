using Microsoft.EntityFrameworkCore;
using Swar.API.Contexts;
using Swar.API.Exceptions;
using Swar.API.Interfaces.Repositories;

namespace Swar.API.Repositories
{
    public abstract class AbstractRepository<K, T> : IRepository<K, T> where T : class
    {
        protected readonly SwarContext _context;
        protected readonly DbSet<T> _dbSet;

        public AbstractRepository(SwarContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public async virtual Task<T> Add(T item)
        {
            try
            {
                var result = await _dbSet.AddAsync(item);
                await _context.SaveChangesAsync();
                return result.Entity;
            }
            catch (DbUpdateException)
            {
                throw new UnableToAddException();
            }
        }

        public async virtual Task<T> GetById(K key)
        {
            T result = await _dbSet.FindAsync(key);
            return result;
        }

        public async virtual Task<T> Update(T item)
        {
            _dbSet.Attach(item);
            _context.Entry(item).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return item;
        }

        public async virtual Task<T> Delete(K key)
        {
            T result = await GetById(key);
            if (result == null)
            {
                throw new EntityNotFoundException();
            }
            _dbSet.Remove(result);
            await _context.SaveChangesAsync();
            return result;
        }

        public async virtual Task<IEnumerable<T>> GetAll()
        {
            return await _dbSet.ToListAsync();
        }
    }
}
