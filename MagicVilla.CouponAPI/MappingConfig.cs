using AutoMapper;
using MagicVilla.CouponAPI.Models;
using MagicVilla.CouponAPI.Models.DTO;

namespace MagicVilla.CouponAPI
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<Coupon, CouponCreateDTO>().ReverseMap();
            CreateMap<Coupon, CouponUpdateDTO>().ReverseMap();
            CreateMap<Coupon, CouponDTO>().ReverseMap();
            CreateMap<LocalUser, UserDTO>().ReverseMap();
        }
    }
}
