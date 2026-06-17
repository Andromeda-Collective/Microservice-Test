using User_Service.DTOs.AuthDTOs;

namespace User_Service.Services.Interfaces
{
    public interface IUserService
    {
        Task AddUserAsync(AuthRegisterDto user);
        Task<bool> ExistByUsernameAsync(string username);
        Task<bool> CheckUserPasswordAsync(AuthLoginDto user);
        Task<AuthLoginResponseDto> LoginAsync(AuthLoginDto user);
    }
}
