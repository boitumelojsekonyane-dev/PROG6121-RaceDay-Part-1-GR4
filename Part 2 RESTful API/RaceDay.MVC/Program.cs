
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Register MVC controllers and views
builder.Services.AddControllersWithViews();

// Configure Event API service


// Configure Event API service
builder.Services.AddHttpClient<
    RaceDay.MVC.Services.EventApiService>(client =>
    {
        var apiUrl = builder.Configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException(
                "API BaseUrl is not configured.");

        client.BaseAddress = new Uri(apiUrl);
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        var handler = new HttpClientHandler();

        if (builder.Environment.IsDevelopment())
        {
            // Local development only: accept the API development certificate.
            handler.ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
        }

        return handler;
    });


// Configure Authentication API service
builder.Services.AddHttpClient<
    RaceDay.MVC.Services.AuthApiService>(client =>
    {
        var apiUrl = builder.Configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException(
                "API BaseUrl is not configured.");

        client.BaseAddress = new Uri(apiUrl);
    });


builder.Services.AddHttpClient<
    RaceDay.MVC.Services.EnrolmentApiService>(client =>
    {
        var apiUrl = builder.Configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException(
                "API BaseUrl is not configured.");

        client.BaseAddress = new Uri(apiUrl);
    });


// Configure cookie authentication
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy =
            CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = false;
    });

// Enable authorisation
builder.Services.AddAuthorization();

// Configure session management
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy =
        CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// Read JWT validation settings
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT signing key is missing.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException(
        "JWT issuer is missing.");

var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException(
        "JWT audience is missing.");

// Prepare token validation parameters
var tokenValidationParameters =
    new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Convert.FromBase64String(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        RequireExpirationTime = true,
        RequireSignedTokens = true,
        ClockSkew = TimeSpan.FromMinutes(1)
    };

// Register token validation configuration
builder.Services.AddSingleton(
    tokenValidationParameters);

var app = builder.Build();

// Configure error handling
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Authentication must run before authorisation
app.UseAuthentication();

// Enable sessions
app.UseSession();

app.UseAuthorization();

// Configure MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
