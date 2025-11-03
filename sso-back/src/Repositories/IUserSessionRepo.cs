using sso_back.Entities;

namespace sso_back.Repositories;

public interface IUserSessionRepo
{
    public Task<UserSession> CreateSession(UserSession session);
    public Task<UserSession?> GetSessionById(string sessionId);
}