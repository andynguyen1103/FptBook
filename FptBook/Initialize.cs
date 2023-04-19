using FptBook.Areas.Identity.Data;
using FptBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace FptBook;
public static class RoleNames
{
    public const string Administrator = "Administrator";
    public const string StoreManager = "StoreManager";
    public const string Customer = "Customer";
}
public static class Initialize
{
    public static async Task CreateRoles(IApplicationBuilder app)
    {
        //adding customs roles : Question 1
        var scope = app.ApplicationServices.CreateScope();
        //Resolve ASP .NET Core Identity with DI help
        var userManager = (UserManager<FptBookUser>)scope.ServiceProvider.GetService(typeof(UserManager<FptBookUser>));
        var roleManager = (RoleManager<IdentityRole>)scope.ServiceProvider.GetService(typeof(RoleManager<IdentityRole>));
        var context = (FptBookIdentityDbContext)scope.ServiceProvider.GetService(typeof(FptBookIdentityDbContext));

        if (roleManager.Roles.IsNullOrEmpty())
        {
            var roleNames = typeof(RoleNames).GetFields().ToList();
            IdentityResult roleResult;

            foreach (var r  in roleNames)
            {
                var roleName = r.GetRawConstantValue().ToString();
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 2
                    roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        if (!userManager.GetUsersInRoleAsync(RoleNames.Administrator).Result.IsNullOrEmpty())
        {
            return;
        }
        //Here you could create a super user who will maintain the web app
        var adminUser = new FptBookUser()
        {
            UserName = "admin@fptbook.com",
            Email = "admin@fptbook.com",
            EmailConfirmed = true
        };
        
        var createAdminUser = await userManager.CreateAsync(adminUser,"admin1234");
        if (createAdminUser.Succeeded)
        {
            //here we tie the new user to the role : Question 3
            await userManager.AddToRoleAsync(adminUser, RoleNames.Administrator);

        }

        await context.SaveChangesAsync();

    }
}