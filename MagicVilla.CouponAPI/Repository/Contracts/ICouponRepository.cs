﻿using MagicVilla.CouponAPI.Models;

namespace MagicVilla.CouponAPI.Repository.Contracts
{
    public interface ICouponRepository : IRepositoryBase<Coupon>
    {
        Task<Coupon> GetAsync(string text);
        Task<bool> ExistsAsync(string text);
    }
}
