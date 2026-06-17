using Microsoft.EntityFrameworkCore;
using User_Service.Common.Utilities.Interfaces;
using User_Service.Data;
using User_Service.DTOs.AuthDTOs;
using User_Service.Entities;
using User_Service.Services.Interfaces;

namespace User_Service.Services.Implementations
{
    public class UserService : IUserService
    {
        private ApplicationDbContext _dbContext;
        private readonly ITokenHelper _tokenHelper;
        private readonly ITokenService _tokenService;
        public UserService(ApplicationDbContext dbContext, ITokenHelper tokenHelper, ITokenService tokenService)
        {
            _dbContext = dbContext;
            _tokenHelper = tokenHelper;
            _tokenService = tokenService;
        }


        public async Task AddUserAsync(AuthRegisterDto user)
        {
            var newUser = new User
            {
                Id = Guid.NewGuid(),
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                CreatedAt = DateTime.UtcNow,
                HashPassword = BCrypt.Net.BCrypt.HashPassword(user.Password)
            };


            await _dbContext.Users.AddAsync(newUser);

            await _dbContext.SaveChangesAsync();
        }

        public async Task<bool> CheckUserPasswordAsync(AuthLoginDto user)
        {
            var hashPassword = await _dbContext.Users
                .Where(x => x.Username == user.Username)
                .Select(x => x.HashPassword)
                .FirstOrDefaultAsync();


            if (hashPassword == null)
                return false;


            return BCrypt.Net.BCrypt.Verify(
                user.Password,
                hashPassword
            );
        }

        public async Task<bool> ExistByUsernameAsync(string username)
        {
            return await _dbContext.Users
                .AnyAsync(x => x.Username == username);
        }


        public async Task<AuthLoginResponseDto> LoginAsync(AuthLoginDto user)
        {
            var userId = await _dbContext.Users
                .Where(x => x.Username == user.Username)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();


            var token = _tokenHelper.GenerateToken(
                userId.ToString()
            );


            var refreshToken = await _tokenService
                .GetAndSaveRefreshTokenAsync(userId);


            return new AuthLoginResponseDto
            {
                JwtToken = token,
                UserId = userId.ToString(),
                UserName = user.Username,
                RefreshToken = refreshToken
            };
        }

    }
}
