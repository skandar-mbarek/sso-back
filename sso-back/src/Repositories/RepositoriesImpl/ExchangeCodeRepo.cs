using Microsoft.EntityFrameworkCore;
using sso_back.Data;
using sso_back.Entities;

namespace sso_back.Repositories.RepositoriesImpl;

public class ExchangeCodeRepo : IExchangeCodeRepo
{
    private readonly ApplicationDbContext _context;
    public ExchangeCodeRepo(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<ExchangeCode> CreateExchangeCode(ExchangeCode code)
    {
        var savedCode = await _context.ExchangeCodes.AddAsync(code);
        await _context.SaveChangesAsync();
        return savedCode.Entity ;
    }

    public async Task<ExchangeCode?> GetExchangeCodeByCode(string code)
    {
        var exchangeCode = await _context.ExchangeCodes.FirstOrDefaultAsync(x => x.Code == code);
        return exchangeCode;
    }
}