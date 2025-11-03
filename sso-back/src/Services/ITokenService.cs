using System.Security.Claims;

namespace sso_back.Services;

public interface ITokenService
{
    string GenerateAccessToken(IEnumerable<Claim> claims);
    string GenerateRefreshToken(IEnumerable<Claim> claims);
    string GenerateSimpleToken(IEnumerable<Claim> claims);
    IEnumerable<Claim> GetClaimsFromToken(string token);

    public IEnumerable<Claim> ValidateToken(string token);

}