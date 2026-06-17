
using User_Service.DTOs.AuthDTOs;

namespace User_Service.Services.Interfaces;

public interface ITokenService
{
    Task<string> GetAndSaveRefreshTokenAsync(Guid userId);
    Task<Guid?> GetUserIdByValidTokenAsync(string token);
    Task<AuthRefreshDto> RefreshAsync(Guid userId);
}