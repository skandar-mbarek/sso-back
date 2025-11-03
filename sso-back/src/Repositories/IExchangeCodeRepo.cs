using sso_back.Entities;

namespace sso_back.Repositories;

public interface IExchangeCodeRepo
{
    public Task<ExchangeCode> CreateExchangeCode(ExchangeCode code);
    public Task<ExchangeCode?> GetExchangeCodeByCode(string code);
}