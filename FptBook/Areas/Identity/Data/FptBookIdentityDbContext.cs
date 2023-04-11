using FptBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.Identity.Data;

public class FptBookIdentityDbContext : IdentityDbContext<ApplicationUser>
{
    public FptBookIdentityDbContext(DbContextOptions<FptBookIdentityDbContext> options)
        : base(options)
    {
        //Eliminate aspnet before the identity 
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            if (entityType.GetTableName().StartsWith("AspNet"))
            {
                entityType.SetTableName(entityType.GetTableName().Substring(6)); //Remove the AspNet
            }
        }
    }
}
