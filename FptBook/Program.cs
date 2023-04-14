using System.Configuration;
using FptBook;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FptBook.Areas.Identity.Data;
using FptBook.Mail;
using FptBook.Models;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("FptBookIdentityDbContextConnection") ?? throw new InvalidOperationException("Connection string 'FptBookIdentityDbContextConnection' not found.");
builder.Services.AddDbContext<FptBookIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

//add Identity services
builder.Services.AddDefaultIdentity<FptBookUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<FptBookIdentityDbContext>()
    .AddDefaultTokenProviders();

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

// Cookie configuration
builder.Services.ConfigureApplicationCookie (options => {
    // options.Cookie.HttpOnly = true;  
    options.ExpireTimeSpan = TimeSpan.FromMinutes (30);
    options.LoginPath = $"/login/"; // Url đến trang đăng nhập
    options.LogoutPath = $"/logout/";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied"; // Trang khi User bị cấm truy cập
});

builder.Services.Configure<SecurityStampValidatorOptions> (options => {
    // Check user every 5 secs
    options.ValidationInterval = TimeSpan.FromSeconds (5);
});

builder.Services.Configure<RouteOptions> (options => {
    options.AppendTrailingSlash = false; // add "/" that the end
    options.LowercaseUrls = true; // Lower case url
    options.LowercaseQueryStrings = false; // 
});

// Add SendMailService to pipeline
builder.Services.AddOptions();
//get the mail setting from app setting
builder.Services.Configure<MailSettings> (builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<IEmailSender, SendMailService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddRazorPages();

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

// app.UseSession();   

app.UseRouting();

await Initialize.CreateRoles(app);

app.UseAuthentication();;
app.UseAuthorization();


app.UseEndpoints (endpoints => {
    
    // Url sẽ là /Area/Controller/Action 
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.MapRazorPages();

app.Run();

app.Run (async (HttpContext context) => {
    context.Response.StatusCode = StatusCodes.Status404NotFound;
    await context.Response.WriteAsync ("Page not found!");
});



