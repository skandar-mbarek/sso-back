namespace sso_back.Dtos.ResponseDtos;

public class LoginRes
{
    public string SessionToken { get; set; }
    public string RedirectUrl { get; set; }
}