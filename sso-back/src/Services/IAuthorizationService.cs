using sso_back.Dtos.RequestDtos;
using sso_back.Dtos.ResponseDtos;

namespace sso_back.Services;

public interface IAuthorizationService
{
    Task<LoginRes> Login(LoginReq req);

    Task<string> AuthorizeClient(string token);
    public Task<ExchangeTokenRes> ExchangeToken(string code);
    
    public Task<ExchangeTokenRes?> RefreshToken(string refreshToken);

    public Task<LoginRes?> ValidateSessionToken(ValidateSessionReq req);
}