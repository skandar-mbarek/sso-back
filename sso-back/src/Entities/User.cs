using Microsoft.AspNetCore.Identity;

namespace sso_back.Entities;

public class User : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public List<UserSession> Sessions { get; set; } = new();

}