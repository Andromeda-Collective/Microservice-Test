

using User_Service.AuthEntities;

namespace User_Service.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string HashPassword { get; set; } = null!;
    public DateTime CreatedAt { get; set; }


    public List<Token> Tokens { get; set; } = new();
}

