using sso_back.Data;
using sso_back.Entities;

namespace sso_back.Repositories.RepositoriesImpl;

public class ClientRepo : IClientRepo
{
    
    private readonly ApplicationDbContext _context;
    public ClientRepo(ApplicationDbContext context)
    {
        _context = context;
    }
    
    
    public async Task<Client> CreateClient(Client client)
    {
        var savedClient = await _context.Clients.AddAsync(client);
        await _context.SaveChangesAsync();
        return savedClient.Entity ;
    }

    public async Task<Client?> GetClientById(string id)
    {
        var client = await _context.Clients.FindAsync(id);
        return client;
    }
}