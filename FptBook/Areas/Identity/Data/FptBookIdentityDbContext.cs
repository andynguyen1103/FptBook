using FptBook.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FptBook.Areas.Identity.Data;

public class FptBookIdentityDbContext : IdentityDbContext<FptBookUser>
{
    public FptBookIdentityDbContext(DbContextOptions<FptBookIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating (builder); 
        
        // Eliminate AspNet prefix on tables
        foreach (var entityType in builder.Model.GetEntityTypes ()) {
            var tableName = entityType.GetTableName ();
            if (tableName.StartsWith ("AspNet")) {
                entityType.SetTableName (tableName.Substring (6));
            }
        }
        
    }
}
