using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sso_back.Entities;

namespace sso_back.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<ExchangeCode> ExchangeCodes { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.HasDefaultSchema("identity");

            // USER → USER SESSIONS (Cascade)
            builder.Entity<UserSession>()
                .HasOne(us => us.User)
                .WithMany(u => u.Sessions)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // EXCHANGE CODE → USER (Cascade)
            builder.Entity<ExchangeCode>()
                .HasOne(ec => ec.User)
                .WithMany()
                .HasForeignKey(ec => ec.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // EXCHANGE CODE → USER SESSION (Restrict to avoid multiple cascade paths)
            builder.Entity<ExchangeCode>()
                .HasOne(ec => ec.UserSession)
                .WithMany()
                .HasForeignKey(ec => ec.UserSessionId)
                .OnDelete(DeleteBehavior.Restrict);

            // UNIQUE INDEXES
            builder.Entity<Client>()
                .HasIndex(c => c.ClientId)
                .IsUnique();

            builder.Entity<ExchangeCode>()
                .HasIndex(c => c.Code)
                .IsUnique();
        }
    }
}