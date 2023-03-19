using MagicVilla.CouponAPI.Models.DTO;

namespace MagicVilla.CouponAPI.Repository.Contracts
{
    public interface IAuthRepository
    {
        bool IsUniqueUser(string username);
        Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO);
        Task<UserDTO> Register(RegistrationRequestDTO requestDTO);
    }
}
