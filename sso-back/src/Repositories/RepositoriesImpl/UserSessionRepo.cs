using Microsoft.EntityFrameworkCore;
using sso_back.Data;
using sso_back.Entities;

namespace sso_back.Repositories.RepositoriesImpl;

public class UserSessionRepo : IUserSessionRepo
{
    private readonly ApplicationDbContext _context;
    
    public UserSessionRepo(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task<UserSession> CreateSession(UserSession session)
    {
        var savedSession = await _context.UserSessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return savedSession.Entity ;
    }

    public async Task<UserSession?> GetSessionById(string sessionId)
    {
        var session =await _context.UserSessions.FirstOrDefaultAsync(s => s.Id == sessionId);
        return session;
    }
}