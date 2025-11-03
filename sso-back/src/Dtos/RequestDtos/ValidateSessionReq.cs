namespace sso_back.Dtos.RequestDtos;

public class ValidateSessionReq
{
    public string SessionToken { get; set; } = "";
    public string ClientUrl { get; set; } = "";
}