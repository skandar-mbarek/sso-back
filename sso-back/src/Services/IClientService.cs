using sso_back.Dtos.RequestDtos;

namespace sso_back.Services;

public interface IClientService
{
    Task<string> CreateClient(CreateClientReq clientDto);
}