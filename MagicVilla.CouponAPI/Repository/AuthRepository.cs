using AutoMapper;
using MagicVilla.CouponAPI.Data;
using MagicVilla.CouponAPI.Models;
using MagicVilla.CouponAPI.Models.DTO;
using MagicVilla.CouponAPI.Repository.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVilla.CouponAPI.Repository
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext dbContext;
        private readonly IMapper mapper;
        private readonly string secretKey;

        public AuthRepository(ApplicationDbContext dbContext, IMapper mapper, IConfiguration configuration)
        {
            this.dbContext = dbContext;
            this.mapper = mapper;
            secretKey = configuration.GetValue<string>("ApiSettings:secret");
        }

        public async Task<LoginResponseDTO> Login(LoginRequestDTO requestDTO)
        {
            var user = await dbContext.LocalUsers.FirstOrDefaultAsync(q => q.UserName.Equals(requestDTO.UserName));
            if (user == null || user.Password != requestDTO.Password)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, user.Role),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var loginResponse = new LoginResponseDTO
            {
                User = mapper.Map<UserDTO>(user),
                Token = new JwtSecurityTokenHandler().WriteToken(token)
            };

            return loginResponse;
        }

        public bool IsUniqueUser(string username) => !dbContext.LocalUsers.Any(q => q.UserName == username);

        public async Task<UserDTO> Register(RegistrationRequestDTO requestDTO)
        {
            var localUser = new LocalUser
            {
                UserName = requestDTO.UserName,
                Name = requestDTO.Name,
                Password = requestDTO.Password,
                Role = "customer"
            };

            await dbContext.AddAsync(localUser);
            await dbContext.SaveChangesAsync();
            localUser.Password = string.Empty;

            return mapper.Map<UserDTO>(localUser);
        }
    }
}
