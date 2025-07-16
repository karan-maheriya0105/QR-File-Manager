using BrochureAPI.Interfaces;
using BrochureAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BrochureAPI.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var userService = serviceProvider.GetRequiredService<IUserService>();

            // Apply any pending migrations
            await context.Database.MigrateAsync();

            // Check if there are any users
            if (!await context.Users.AnyAsync())
            {
                // Create admin user
                var adminPassword = await userService.HashPasswordAsync("Admin@123");
                var adminUser = new User
                {
                    StrName = "Administrator",
                    StrEmailId = "admin@brochure.com",
                    StrPassword = adminPassword,
                    BolIsAdmin = true
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
} 