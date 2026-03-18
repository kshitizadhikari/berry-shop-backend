using Domain;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Seeders;

public static class Seeder
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        foreach (var role in Enum.GetValues<AppRole>())
        {
            var roleName = role.ToString();
            if (!await roleManager.RoleExistsAsync(roleName))
                await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    public static async Task SeedAdminAsync(UserManager<AppUser> userManager, IConfiguration config)
    {
        var adminSection = config.GetSection("AdminSeed");

        var email = adminSection["Email"] ??
                    throw new InvalidOperationException("AdminSeed:Email is missing from configuration.");
        var password = adminSection["Password"] ??
                       throw new InvalidOperationException("AdminSeed:Password is missing from configuration.");
        var firstName = adminSection["FirstName"] ?? "Admin";
        var lastName = adminSection["LastName"] ?? "User";

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) return;

        var admin = new AppUser
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        var result = await userManager.CreateAsync(admin, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(
                $"Failed to seed admin: {string.Join("; ", result.Errors.Select(e => e.Description))}");

        await userManager.AddToRoleAsync(admin, AppRole.Admin.ToString());
    }
}