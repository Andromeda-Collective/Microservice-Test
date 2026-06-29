using System.ComponentModel.DataAnnotations;

namespace User_Service.DTOs.AuthDTOs;

public class AuthRefreshDto
{
    [Required]
    public required string RefreshToken { get; set; }
}