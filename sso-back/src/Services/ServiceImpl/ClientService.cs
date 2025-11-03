using System.Security.Claims;
using sso_back.Dtos.RequestDtos;
using sso_back.Entities;
using sso_back.Repositories;

namespace sso_back.Services.ServiceImpl;

public class ClientService : IClientService
{
    private readonly IClientRepo _repo;
    private readonly ITokenService _tokenService;


    public ClientService(IClientRepo repo, ITokenService tokenService)
    {
        _repo = repo;
        _tokenService = tokenService;
    }

    public async Task<string> CreateClient(CreateClientReq clientDto)
    {
        var client = new Client
        {
            ClientId = clientDto.ClientId,
            ClientUrl = clientDto.ClientUrl,
        };
        var savedClient = await _repo.CreateClient(client);
        var claims = new List<Claim>
        {
            new Claim("ID", savedClient.Id),
            new Claim("CLIENT_ID",savedClient.ClientId )
        };
        var token = _tokenService.GenerateSimpleToken(claims);
        return token;

    }
}