using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FptBook.Areas.Identity.Data;
var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("FptBookIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'FptBookIdentityDbContextConnection' not found.");

builder.Services.AddDbContext<FptBookIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<FptBookIdentityDbContext>();

// IdentityOptions
builder.Services.Configure<IdentityOptions> (options => {
    // Password configuration
    options.Password.RequireDigit = true; // Require digits
    options.Password.RequireLowercase = false; // Don't require lowercase
    options.Password.RequireNonAlphanumeric = false; // Don't require special character
    options.Password.RequireUppercase = false; // Don't require uppercase
    options.Password.RequiredLength = 3; // Minimum character
    options.Password.RequiredUniqueChars = 1; // Number of unique character

    // Lockout configuration
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Lockout time
    options.Lockout.MaxFailedAccessAttempts = 5; // Login till lockout
    options.Lockout.AllowedForNewUsers = true;

    // User configuration.
    options.User.AllowedUserNameCharacters = // Username characters
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;  // Email là duy nhất

    // Login configuration.
    options.SignIn.RequireConfirmedEmail = true;            // Require confirmed email
    options.SignIn.RequireConfirmedPhoneNumber = false;     // Require confirmed phone number
});

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();;
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();