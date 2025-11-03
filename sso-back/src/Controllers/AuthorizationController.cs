using Microsoft.AspNetCore.Mvc;
using sso_back.Dtos;
using sso_back.Dtos.RequestDtos;
using sso_back.Dtos.ResponseDtos;
using sso_back.Services;

namespace sso_back.Controllers;

[ApiController]
[Route("api/connect")]
public class AuthorizationController(IAuthorizationService authorizationService) : ControllerBase
{

    [HttpGet("authorize")]
    public async Task<IActionResult> AuthorizeClient([FromHeader] string clientToken)
    {
        if (Request.Cookies.TryGetValue("REFRESH_TOKEN", out var refreshToken))
        {
            var response = await authorizationService.RefreshToken(refreshToken);

            if (response == null)
            {
                var redirectUrl = await authorizationService.AuthorizeClient(clientToken);
                throw new BadHttpRequestException("Refresh token could not be retrieved");
            }

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Must be true for Netlify (HTTPS)
                SameSite = SameSiteMode.None, // Required for cross-origin
                Expires = DateTime.UtcNow.AddMonths(3),
                Domain =  ".vercel.app", 
                Path = "/"
            };
            Response.Cookies.Append("REFRESH_TOKEN", response.RefreshToken, cookieOptions);
            return Ok(new { access_token = response.AccessToken });
        }

        var redirectUrl2 = await authorizationService.AuthorizeClient(clientToken);
        throw new UnauthorizedAccessException("Refresh token could not be retrieved");
    }




    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginReq req)
    {
        var response = await authorizationService.Login(req);
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Must be true for Netlify (HTTPS)
            SameSite = SameSiteMode.None, // Required for cross-origin
            Expires = DateTime.UtcNow.AddMonths(3),
            Domain =  ".netlify.app", 
            Path = "/"
        };
        Response.Cookies.Append("SESSION_TOKEN", response.SessionToken, cookieOptions);
        return Ok(new{redirectUrl = response.RedirectUrl});
    }

    [HttpPost("exchange")]
    public async Task<IActionResult> ExchangeToken(string code)
    {
        var response = await authorizationService.ExchangeToken(code);
        
        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true, // Must be true for Netlify (HTTPS)
            SameSite = SameSiteMode.None, // Required for cross-origin
            Expires = DateTime.UtcNow.AddMonths(3),
            Domain =  ".vercel.app", 
            Path = "/"
        };
        Response.Cookies.Append("REFRESH_TOKEN", response.RefreshToken, cookieOptions);
        return Ok(new {access_token = response.AccessToken});

    }
    
    [HttpPost("check-session")]
    public async Task<IActionResult> CheckSession(string clientUrl)
    {
        if (Request.Cookies.TryGetValue("SESSION_TOKEN", out var sessionToken))
        {
            var checkSessionReq = new ValidateSessionReq
            {
                ClientUrl = clientUrl,
                SessionToken = sessionToken
            };
            var response = await authorizationService.ValidateSessionToken(checkSessionReq);
            if (response == null)
            {
                throw new UnauthorizedAccessException("Session expired");
            }
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = true, // Must be true for Netlify (HTTPS)
                SameSite = SameSiteMode.None, // Required for cross-origin
                Expires = DateTime.UtcNow.AddMonths(3),
                Domain =  ".netlify.app", 
                Path = "/"
            };
            Response.Cookies.Append("SESSION_TOKEN", response.SessionToken, cookieOptions);
            return Ok( new {redirectUrl=response.RedirectUrl});
        }
        throw new UnauthorizedAccessException("Not connected");
    }
}