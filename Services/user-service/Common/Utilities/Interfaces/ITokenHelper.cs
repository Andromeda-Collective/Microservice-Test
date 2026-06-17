
namespace User_Service.Common.Utilities.Interfaces;

public interface ITokenHelper
{
    string GenerateToken(string userId);
    string GenerateRefreshToken();
}