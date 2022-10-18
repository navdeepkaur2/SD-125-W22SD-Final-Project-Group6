using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SD_340_W22SD_Final_Project_Group6.Data;

namespace SD_340_W22SD_Final_Project_Group6.Models
{
    public static class SeedData
    {
        public async static Task Initialize(IServiceProvider serviceProvider)
        {
            ApplicationDbContext context
                = new ApplicationDbContext(serviceProvider
                .GetRequiredService<DbContextOptions<ApplicationDbContext>>());

            RoleManager<IdentityRole> roleManager =
                serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            List<string> roles = new List<string>
            {
                "ProjectManager", "Developer", "Admin"
            };

            if (!context.Roles.Any())
            {
                foreach (string role in roles)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
                await context.SaveChangesAsync();
            }

            if (!context.Users.Any())
            {
 
                ApplicationUser seedAdminUser = new ApplicationUser
                {
                    Email = "admin@jello.com",
                    NormalizedEmail = "ADMIN@JELLO.COM",
                    UserName = "admin@jello.com",
                    NormalizedUserName = "ADMIN@JELLO.COM",
                    EmailConfirmed = true,
                };

                var password3 = new PasswordHasher<ApplicationUser>();
                var hashed3 = password3.HashPassword(seedAdminUser, "P@ssW0rd");
                seedAdminUser.PasswordHash = hashed3;
                await userManager.CreateAsync(seedAdminUser);
                await userManager.AddToRoleAsync(seedAdminUser, "Admin");
                await context.SaveChangesAsync();
            }

        }
    }
}
