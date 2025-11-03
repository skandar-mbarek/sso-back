using sso_back.Entities;

namespace sso_back.Repositories;

public interface IClientRepo
{
    Task<Client> CreateClient(Client client);
    Task<Client?> GetClientById(string id);
}