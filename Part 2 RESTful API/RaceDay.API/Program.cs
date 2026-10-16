
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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


// Create one SQL Server connection per request scope.
builder.Services.AddScoped<SqlConnection>(
    _ => new SqlConnection(connectionString));

// Both DbContexts use the same scoped connection.
builder.Services.AddDbContext<RaceDayDbContext>(
    (serviceProvider, options) =>
    {
        var connection =
            serviceProvider.GetRequiredService<SqlConnection>();

        options.UseSqlServer(connection);
    });

builder.Services.AddDbContext<RaceDayIdentityDbContext>(
    (serviceProvider, options) =>
    {
        var connection =
            serviceProvider.GetRequiredService<SqlConnection>();

        options.UseSqlServer(connection);
    });


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



builder.Services.AddScoped<
    RaceDay.API.Services.ParticipantRegistrationService>();

builder.Services.AddScoped<
    RaceDay.API.Services.JwtTokenService>();

// Service for securely provisioning Organiser accounts.
builder.Services.AddScoped<
    RaceDay.API.Services.OrganiserProvisioningService>();



var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT signing key is missing from configuration.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer is missing from configuration.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience is missing from configuration.");

var signingKey = Convert.FromBase64String(jwtKey);

if (signingKey.Length < 32)
{
    throw new InvalidOperationException(
        "JWT signing key must contain at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtIssuer,

                ValidateAudience = true,
                ValidAudience = jwtAudience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(signingKey),

                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1),

                RoleClaimType = "role",
                NameClaimType = "sub"
            };
    });
        

builder.Services.AddAuthorization();

var app = builder.Build();



using (var scope = app.Services.CreateScope())
{
    var roleManager =
        scope.ServiceProvider.GetRequiredService<
            Microsoft.AspNetCore.Identity.RoleManager<
                Microsoft.AspNetCore.Identity.IdentityRole>>();

    if (!await roleManager.RoleExistsAsync("Participant"))
    {
        var result = await roleManager.CreateAsync(
            new Microsoft.AspNetCore.Identity.IdentityRole("Participant"));

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to create Participant role: " +
                string.Join("; ", result.Errors.Select(e => e.Description)));
        }
    }

    // Create the Organiser role if it does not exist.
    if (!await roleManager.RoleExistsAsync("Organiser"))
    {
        var result = await roleManager.CreateAsync(
            new IdentityRole("Organiser"));

        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                "Failed to create Organiser role: " +
                string.Join("; ",
                    result.Errors.Select(e => e.Description)));
        }
    }

}


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
