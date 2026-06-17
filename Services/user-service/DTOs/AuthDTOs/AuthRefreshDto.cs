namespace User_Service.DTOs.AuthDTOs;

public class AuthRefreshDto
{
        public required string JwtToken { get; set; }
        public required string RefreshToken { get; set; }
}