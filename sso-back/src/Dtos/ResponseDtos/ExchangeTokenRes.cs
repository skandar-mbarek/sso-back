namespace sso_back.Dtos.ResponseDtos;

public class ExchangeTokenRes
{
    public string AccessToken { get; set; }=string.Empty;
    public string RefreshToken { get; set; }=string.Empty;
}