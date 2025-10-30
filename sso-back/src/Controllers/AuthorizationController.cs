using Microsoft.AspNetCore.Mvc;
using sso_back.Dtos;

namespace sso_back.Controllers;

[ApiController]
[Route("api/connect")]
public class AuthorizationController : ControllerBase
{
    private const string UserEmail = "admin@bookini.com";
    private const string UserPassword = "admin123";
    private const string ExchangeCode = "123456789";
    private const string UserToken = "EVsMumo9bxcuidwcMf3sGqw8uf%2BYhSM%2Bq28A698vtvyf%2BrEtnOB9STWeCI1OvEQ";
    private const string ClientId1 = "client-app-1"; 
    private const string ClientUrl1 ="http://localhost:3001/";
    private const string ClientId2 = "client-app-2"; 
    private const string ClientUrl2 ="http://localhost:3002/";
    
    [HttpPost("login")]
    public IActionResult Login(LoginReq request)
    {
        if (request.Email == UserEmail && request.Password == UserPassword)
        {
            switch (request.ClientId)
            {
                case ClientId1:
                    return Ok(new{redirectUrl=$"{ClientUrl1}exchange?exchangeCode={ExchangeCode}",token=UserToken});
                case ClientId2 :
                    return Ok(new{redirectUrl=$"{ClientUrl2}exchange?exchangeCode={ExchangeCode}",token=UserToken});
                default:
                    throw new BadHttpRequestException("Invalid client ID");
            }
        }
        throw new UnauthorizedAccessException();
    }

    [HttpPost("callback")]
    public IActionResult Callback(string token ,string clientId)
    {
        if (token == UserToken)
        {
            switch (clientId)
            {
                case ClientId1:
                    return Ok(new{redirectUrl=$"{ClientUrl1}exchange?exchangeCode={ExchangeCode}",token=UserToken});
                case ClientId2 :
                    return Ok(new{redirectUrl=$"{ClientUrl2}exchange?exchangeCode={ExchangeCode}",token=UserToken});
                default:
                    throw new BadHttpRequestException("Invalid client ID");
            }
        }

        throw new UnauthorizedAccessException();
    }
    
    [HttpPost("exchange")]
    public IActionResult Login(string code)
    {
        if (code == ExchangeCode)
        {
            return Ok(new {token = UserToken });
        }
        throw new BadHttpRequestException("Invalid Exchange Code");
    }
    
}