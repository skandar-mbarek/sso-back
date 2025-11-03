using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using sso_back.Dtos.RequestDtos;
using sso_back.Dtos.ResponseDtos;
using sso_back.Entities;
using sso_back.Repositories;

namespace sso_back.Services.ServiceImpl;

public class AuthorizationService : IAuthorizationService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IClientRepo _clientRepo;
    private readonly IUserSessionRepo _userSessionRepo;
    private readonly IExchangeCodeRepo _exchangeCodeRepo;
    
    public AuthorizationService(UserManager<User> userManager, SignInManager<User> signInManager, ITokenService tokenService, IClientRepo clientRepo, IUserSessionRepo userSessionRepo, IExchangeCodeRepo exchangeCodeRepo)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _clientRepo = clientRepo;
        _userSessionRepo = userSessionRepo;
        _exchangeCodeRepo = exchangeCodeRepo;
    }
    
    public async Task<string> AuthorizeClient(string token)
    {
        var claims = _tokenService.ValidateToken(token);
        var clientGuidId = claims.FirstOrDefault(c=>c.Type =="ID")?.Value;
        if (string.IsNullOrEmpty(clientGuidId))
        {
            throw new SecurityTokenArgumentException("Invalid token");
        }
        var client = await _clientRepo.GetClientById(clientGuidId);
        if (client == null)
        {
            throw new SecurityTokenArgumentException("Invalid token");
        }
        return $"https://main-sso-front.netlify.app?client_url={client.ClientUrl}&client_id={client.Id}";
    }
    
    public async Task<LoginRes> Login(LoginReq req)
    {
        var user = await _userManager.FindByEmailAsync(req.Email);
        if (user == null)
            throw new UnauthorizedAccessException("User not found");

        var result = await _signInManager.CheckPasswordSignInAsync(user, req.Password, false);
        if (!result.Succeeded)
            throw new UnauthorizedAccessException("Verify your credentials");

        var session = new UserSession
        {
            UserId = user.Id,
            IsActive = true
        };
        var savedSession = await _userSessionRepo.CreateSession(session);

        // ✅ Generate secure random code
        var bytes = RandomNumberGenerator.GetBytes(32);
        var code = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        var exchangeCode = new ExchangeCode
        {
            Code = code,
            IsUsed = false,
            UserId = user.Id,
            UserSessionId = savedSession.Id,
            ExpiredAt = DateTime.Now.AddMinutes(10)
        };

        var savedCode = await _exchangeCodeRepo.CreateExchangeCode(exchangeCode);

        var claims = new List<Claim>
        {
            new Claim("Session_ID", savedSession.Id)
        };
        var token = _tokenService.GenerateSimpleToken(claims);

        return new LoginRes
        {
            SessionToken = token,
            RedirectUrl = $"{req.ClientUrl}/exchange?code={savedCode.Code}"
        };
    }

    public async Task<ExchangeTokenRes> ExchangeToken(string code)
    {
        var exchangeCode = await _exchangeCodeRepo.GetExchangeCodeByCode(code);
        if (exchangeCode == null)
            throw new UnauthorizedAccessException("you don't have access to exchange");
        
        if (exchangeCode.IsUsed)
            throw new UnauthorizedAccessException("this code is already used");
        
        if (exchangeCode.ExpiredAt<DateTime.Now)
            throw new UnauthorizedAccessException("this code is expired");
        
        var claims = new List<Claim>
        {
            new Claim("Session_ID", exchangeCode.UserSessionId),
            new Claim("User_ID", exchangeCode.UserId),
        };


        var accessToken = _tokenService.GenerateAccessToken(claims);
        var refreshToken = _tokenService.GenerateRefreshToken(claims);

        return new ExchangeTokenRes
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public async Task<ExchangeTokenRes?> RefreshToken(string refreshToken)
    {
        var claims = _tokenService.ValidateToken(refreshToken);
        var sessionId = claims.FirstOrDefault(c=>c.Type =="Session_ID")?.Value;
        if (string.IsNullOrEmpty(sessionId))
        {
            return null;
        }

        var session = await _userSessionRepo.GetSessionById(sessionId);
        if (session == null)
            return null;
        
        if (!session.IsActive)
            return null;
        
        
        var newClaims = new List<Claim>
        {
            new Claim("Session_ID", session.Id),
            new Claim("User_ID", session.UserId),
        };
        
        var newAccessToken = _tokenService.GenerateAccessToken(newClaims);  
        var newRefreshToken = _tokenService.GenerateRefreshToken(newClaims);
        
        return new ExchangeTokenRes
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken
        };
    }

    public async Task<LoginRes?> ValidateSessionToken(ValidateSessionReq req) 
    {
        var claims = _tokenService.ValidateToken(req.SessionToken);
        var sessionId = claims.FirstOrDefault(c=>c.Type=="Session_ID")?.Value;
        if (string.IsNullOrEmpty(sessionId))
        {
            return null;
        }
        
        var session = await _userSessionRepo.GetSessionById(sessionId);
        if (session == null) return null;
        
        if (!session.IsActive)
            return null;
        
        // ✅ Generate secure random code
        var bytes = RandomNumberGenerator.GetBytes(32);
        var code = Convert.ToBase64String(bytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");

        
        var exchangeCode = new ExchangeCode
        {
            Code = code,
            IsUsed = false,
            UserId = session.UserId,
            UserSessionId = session.Id,
            ExpiredAt = DateTime.Now.AddMinutes(10)
        };
        
        var savedCode = await _exchangeCodeRepo.CreateExchangeCode(exchangeCode);

        var newClaims = new List<Claim>
        {
            new Claim("Session_ID", session.Id)
        };
        var token = _tokenService.GenerateSimpleToken(newClaims);

        return new LoginRes
        {
            SessionToken = token,
            RedirectUrl = $"{req.ClientUrl}/exchange?code={savedCode.Code}"
        };
        
    }
    
}