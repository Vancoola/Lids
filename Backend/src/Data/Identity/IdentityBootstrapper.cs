using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Data.Identity;

public static class IdentityBootstrapper
{
    private const string SuperAdminRole = "SuperAdmin";

    public static async Task SeedSuperAdminAsync(
        this IServiceProvider services,
        IConfiguration configuration)
    {
        if (!configuration.GetValue<bool>(
                "BootstrapAdmin:Enabled"))
            return;

        using var scope = services.CreateScope();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<ApplicationUser>>();

        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        if (!await roleManager.RoleExistsAsync(SuperAdminRole))
        {
            var result = await roleManager.CreateAsync(
                new IdentityRole<Guid>(SuperAdminRole));

            EnsureSucceeded(result);
        }

        var admins = await userManager
            .GetUsersInRoleAsync(SuperAdminRole);

        if (admins.Count > 0)
            return;

        var email = configuration["BootstrapAdmin:Email"];
        var password = configuration["BootstrapAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
            throw new InvalidOperationException(
                "Bootstrap admin credentials are missing.");

        var existing = await userManager.FindByEmailAsync(email);

        if (existing is not null)
            throw new InvalidOperationException(
                "Bootstrap admin email is already registered.");

        var admin = new ApplicationUser
        {
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        var createResult = await userManager.CreateAsync(
            admin,
            password);

        EnsureSucceeded(createResult);

        var roleResult = await userManager.AddToRoleAsync(
            admin,
            SuperAdminRole);

        EnsureSucceeded(roleResult);
    }

    private static void EnsureSucceeded(IdentityResult result)
    {
        if (result.Succeeded)
            return;

        var errors = string.Join(
            "; ",
            result.Errors.Select(x => x.Description));

        throw new InvalidOperationException(errors);
    }
}