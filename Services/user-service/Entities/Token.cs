using User_Service.Entities;

namespace User_Service.AuthEntities;

public class Token
{
    public int Id { get; set; }
    public Guid UserId { get; set; }
    public string HashRefreshToken { get; set; } = null!;
    public bool IsValid { get; set; }
    public DateTime ExpireTime { get; set; }


    public User User { get; set; } = null!;
}
