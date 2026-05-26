using Microsoft.AspNetCore.Identity;

namespace _10xPV.Data;

public static class SeedData
{
    public static async Task EnsureAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        var adminEmail = config["AdminEmail"] 
            ?? throw new InvalidOperationException("AdminEmail not configured.");
        var adminPassword = config["AdminPassword"] 
            ?? throw new InvalidOperationException("AdminPassword not configured.");

        var existingUser = await userManager.FindByEmailAsync(adminEmail);
        if (existingUser is null)
        {
            var user = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(user, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception($"Failed to seed admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }
    }
}