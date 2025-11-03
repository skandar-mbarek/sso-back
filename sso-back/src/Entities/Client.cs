namespace sso_back.Entities;

public class Client : BaseEntity
{
    public required string ClientId { get; set; }
    public required string ClientUrl { get; set; }
}