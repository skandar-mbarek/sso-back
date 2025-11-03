namespace sso_back.Entities;

public class ExchangeCode : BaseEntity
{
    public required string Code { get; set; }
    
    public bool IsUsed { get; set; } = false;
    public DateTime ExpiredAt { get; set; }
    

    public required string UserId { get; set; }
    public User? User { get; set; }
    public required string UserSessionId { get; set; }
    public UserSession? UserSession { get; set; }
}