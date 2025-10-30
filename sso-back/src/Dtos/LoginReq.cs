namespace sso_back.Dtos;

public class LoginReq
{
    public string ClientId { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
}