using System.ComponentModel.DataAnnotations;

namespace User_Service.DTOs.AuthDTOs;

public class AuthLoginDto
{
    [Required]
    [MaxLength(50)]
    [MinLength(3)]
    [RegularExpression(@"^[A-Za-z][A-Za-z0-9_]*$")]
    public required string Username { get; set; }

    [Required]
    [MaxLength(20)]
    [MinLength(8)]
    public required string Password { get; set; }
}
