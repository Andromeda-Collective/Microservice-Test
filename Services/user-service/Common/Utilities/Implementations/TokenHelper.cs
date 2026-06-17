using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using User_Service.Common.Utilities.Interfaces;

namespace User_Service.Common.Utilities.Implementations;

public class TokenHelper : ITokenHelper
{
    private readonly IConfiguration _configuration;
    public TokenHelper(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(string userId)
    {
        var claims = new[]
{
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

        var jwtConfig = _configuration.GetSection("JWTConfig");
        var issuerKeyBytes = Encoding.UTF8.GetBytes(jwtConfig["SecretKey"]!);

        var securityKey = new SymmetricSecurityKey(issuerKeyBytes);
        var credential = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var jwtToken = new JwtSecurityToken(
            signingCredentials: credential,
            issuer: jwtConfig["Issuer"],
            audience: jwtConfig["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(jwtConfig["TimeoutInMin"]))
        );

        var handler = new JwtSecurityTokenHandler();
        return handler.WriteToken(jwtToken);
    }


    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

}
