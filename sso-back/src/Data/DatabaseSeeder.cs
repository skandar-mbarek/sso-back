using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using sso_back.Entities;

namespace sso_back.Data;

public class DatabaseSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await context.Database.MigrateAsync();

        var email = "admin@bookini.com";
        var existingUser = await userManager.FindByEmailAsync(email);

        if (existingUser == null)
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = "Admin",
                LastName = "Bookini",
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, "Admin@12345");
            if (result.Succeeded)
            {
                Console.WriteLine("✅ Default admin user created.");
            }
            else
            {
                Console.WriteLine("❌ Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }
    }
}