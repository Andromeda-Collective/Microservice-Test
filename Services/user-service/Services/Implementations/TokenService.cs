using Microsoft.EntityFrameworkCore;
using User_Service.AuthEntities;
using User_Service.Common.Utilities.Implementations;
using User_Service.Common.Utilities.Interfaces;
using User_Service.Data;
using User_Service.DTOs.AuthDTOs;
using User_Service.Services.Interfaces;

namespace User_Service.Services.Implementations;

public class TokenService : ITokenService
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ITokenHelper _tokenHelper;
    public TokenService(ApplicationDbContext dbContext, ITokenHelper tokenHelper)
    {
        _dbContext = dbContext;
        _tokenHelper = tokenHelper;
    }

    public async Task<Guid?> GetUserIdByValidTokenAsync(string token)
    {
        var hashToken = HashHelper.Hash(token);

        var userId = await _dbContext.Tokens
            .Where(x =>
                x.HashRefreshToken == hashToken &&
                x.IsValid &&
                x.ExpireTime >= DateTime.UtcNow)
            .Select(x => (Guid?)x.UserId)
            .FirstOrDefaultAsync();

        return userId;
    }


    public async Task<AuthRefreshResponseDto> RefreshAsync(Guid userId)
    {
        var jwtToken = _tokenHelper.GenerateToken(userId.ToString());

        var refreshToken = await GetAndSaveRefreshTokenAsync(userId);

        return new AuthRefreshResponseDto
        {
            JwtToken = jwtToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<string> GetAndSaveRefreshTokenAsync(Guid userId)
    {
        var refreshToken = _tokenHelper.GenerateRefreshToken();

        var hashRefreshToken = HashHelper.Hash(refreshToken);

        var tokenEntity = await _dbContext.Tokens
            .FirstOrDefaultAsync(x => x.UserId == userId);


        if (tokenEntity != null)
        {
            tokenEntity.HashRefreshToken = hashRefreshToken;
            tokenEntity.IsValid = true;
            tokenEntity.ExpireTime = DateTime.UtcNow.AddDays(3);
        }
        else
        {
            tokenEntity = new Token
            {
                UserId = userId,
                HashRefreshToken = hashRefreshToken,
                IsValid = true,
                ExpireTime = DateTime.UtcNow.AddDays(3)
            };

            await _dbContext.Tokens.AddAsync(tokenEntity);
        }


        await _dbContext.SaveChangesAsync();


        return refreshToken;
    }
}