using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgrammeTracker.Data;
using ProgrammeTracker.Models;
using ProgrammeTracker.Services;

var builder = WebApplication.CreateBuilder(args);

// ── Database ──
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ── Identity ──
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.SlidingExpiration = true;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

// ── Services ──
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<ExcelImportService>();
builder.Services.AddSingleton<NotificationBackgroundService>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<NotificationBackgroundService>());

// ── MVC ──
builder.Services.AddControllersWithViews();

var app = builder.Build();

// ── Middleware ──
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

// ── Seed & Migrate on startup ──
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

    // Apply pending migrations
    await db.Database.MigrateAsync();

    // Seed roles
    foreach (var role in Roles.All)
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole<int>(role));
    }

    // Seed default admin if no users exist
    if (!userManager.Users.Any())
    {
        var admin = new ApplicationUser
        {
            FirstName = "System",
            LastName = "Admin",
            Email = "admin@programmetracker.local",
            UserName = "admin@programmetracker.local",
            Role = Roles.Admin,
            IsActive = true
        };
        var result = await userManager.CreateAsync(admin, "Admin@123!");
        if (result.Succeeded)
            await userManager.AddToRoleAsync(admin, Roles.Admin);
    }

    // Excel import (first launch only)
    var importService = scope.ServiceProvider.GetRequiredService<ExcelImportService>();
    var importPath = Path.Combine(app.Environment.ContentRootPath, "Programme_Tracker.xlsx");
    await importService.RunIfNeededAsync(importPath);
}

// ── Routes ──
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
