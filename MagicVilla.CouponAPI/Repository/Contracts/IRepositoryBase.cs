namespace MagicVilla.CouponAPI.Repository.Contracts
{
    public interface IRepositoryBase<T> where T : class
    {
        Task<ICollection<T>> GetAllAsync();
        Task<T> GetAsync(int id);
        Task CreateAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task SaveAsync();
    }
}
