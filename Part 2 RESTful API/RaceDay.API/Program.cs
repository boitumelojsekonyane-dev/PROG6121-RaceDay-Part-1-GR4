
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.Identity;

var builder = WebApplication.CreateBuilder(args);

// Register API controllers
builder.Services.AddControllers();

// Register OpenAPI
builder.Services.AddOpenApi();

// Get the existing RACEDAY database connection
var connectionString = builder.Configuration
    .GetConnectionString("RaceDayConnection")
    ?? throw new InvalidOperationException(
        "RaceDayConnection was not found.");

// Existing RACEDAY database context
builder.Services.AddDbContext<RaceDayDbContext>(options =>
    options.UseSqlServer(connectionString));

// New authentication database context
builder.Services.AddDbContext<RaceDayIdentityDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDataProtection();

// Configure ASP.NET Core Identity
builder.Services
    .AddIdentityCore<RaceDayUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 10;

        options.User.RequireUniqueEmail = true;

        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.AllowedForNewUsers = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<RaceDayIdentityDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure development services
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// HTTPS redirection
app.UseHttpsRedirection();

// Authentication middleware
app.UseAuthentication();

// Authorisation middleware
app.UseAuthorization();

// Map API controllers
app.MapControllers();

app.Run();
