using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MSGM.Core;
using Serilog;
using System;
using System.Threading.Tasks;

public static class DbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IunitOfWork>();

        var roleName = "Admin";
        var userEmail = "admin@gmail.com";
        var userPassword = "Admin@123";

        var roleExists = await unitOfWork.RoleManager.RoleExistsAsync(roleName);
        if (!roleExists)
        {
            await unitOfWork.RoleManager.CreateAsync(new IdentityRole(roleName));
        }

        var user = await unitOfWork.UserManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            user = new IdentityUser
            {
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true
            };

            var result = await unitOfWork.UserManager.CreateAsync(user, userPassword);
            if (result.Succeeded)
            {
                await unitOfWork.UserManager.AddToRoleAsync(user, roleName);
            }
            else
            {
                Log.Error("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception("User creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));                
            }
        }

        await unitOfWork.CompleteAsync(); // Save if needed (not strictly needed for Identity, but good practice)
    }
}