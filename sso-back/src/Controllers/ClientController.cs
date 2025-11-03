using Microsoft.AspNetCore.Mvc;
using sso_back.Dtos.RequestDtos;
using sso_back.Services;

namespace sso_back.Controllers;



[ApiController]
[Route("api/client")]
public class ClientController(IClientService  clientService) : ControllerBase
{
    [HttpPost("create")]
    public async Task<IActionResult> CreateToken(CreateClientReq request)
    {
        var token = await clientService.CreateClient(request);
        return Ok(new {token=token});
    }
    
}