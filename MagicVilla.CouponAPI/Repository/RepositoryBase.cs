using MagicVilla.CouponAPI.Data;
using MagicVilla.CouponAPI.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.CouponAPI.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected readonly ApplicationDbContext dbContext;
        protected readonly DbSet<T> dbSet;

        public RepositoryBase(ApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
            dbSet = dbContext.Set<T>();
        }

        public async Task CreateAsync(T entity) => await dbSet.AddAsync(entity);

        public async Task<ICollection<T>> GetAllAsync() => await dbSet.ToListAsync();

        public async Task<T> GetAsync(int id) => await dbSet.FindAsync(id);

        public Task RemoveAsync(T entity)
        {
            dbSet.Remove(entity);
            return Task.CompletedTask;
        }

        public async Task SaveAsync()
        {
            await dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(T entity)
        {
            dbSet.Update(entity);
            return Task.CompletedTask;
        }
    }
}
