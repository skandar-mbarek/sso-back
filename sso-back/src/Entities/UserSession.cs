namespace sso_back.Entities;

public class UserSession : BaseEntity
{
    
    public bool IsActive { get; set; } = true;
    public required string UserId { get; set; }
    public User? User { get; set; }
    
}