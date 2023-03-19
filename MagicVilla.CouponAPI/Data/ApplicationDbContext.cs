﻿using MagicVilla.CouponAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla.CouponAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Coupon>().HasData(
                new Coupon { Id = 1, Name = "10OFF", Percent = 10, IsActive = true },
                new Coupon { Id = 2, Name = "20OFF", Percent = 20, IsActive = true }
            );
        }

        public DbSet<Coupon> Coupons { get; set; }
    }
}
