using MagicVilla.CouponAPI.Models;

namespace MagicVilla.CouponAPI.Repository.Contracts
{
    public interface ICouponRepository : IRepositoryBase<Coupon>
    {
        Task<Coupon> GetAsync(string text);
        Task<IEnumerable<Coupon>> GetSpecialAsync(string text, int pageSize, int currentPage);
        Task<bool> ExistsAsync(string text);
    }
}
