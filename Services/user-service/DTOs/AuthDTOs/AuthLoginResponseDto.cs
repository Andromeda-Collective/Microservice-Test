namespace User_Service.DTOs.AuthDTOs;

public class AuthLoginResponseDto
{
    public required string UserId { get; set; }
    public required string JwtToken { get; set; }
    public required string UserName { get; set; }
    public required string RefreshToken { get; set; }
}

