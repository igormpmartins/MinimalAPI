using MagicVilla.CouponAPI.Data;
using MagicVilla.CouponAPI.Models;
using MagicVilla.CouponAPI.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.CouponAPI.Repository
{
    public class CouponRepository : RepositoryBase<Coupon>, ICouponRepository
    {
        public CouponRepository(ApplicationDbContext dbContext) : base(dbContext) { }

        public async Task<bool> ExistsAsync(string text) 
            => await dbSet.AnyAsync(q => q.Name.ToLower() == text.ToLower());

        public Task<Coupon> GetAsync(string text) 
            => dbSet.Where(q => q.Name.ToLower() == text.ToLower()).FirstOrDefaultAsync();

        public async Task<IEnumerable<Coupon>> GetSpecialAsync(string text, int pageSize, int currentPage)
            => await dbSet.Where(q => q.Name.ToLower().Contains(text))
            .Skip(pageSize * (currentPage -1)).Take(pageSize).ToListAsync();
    }
}
