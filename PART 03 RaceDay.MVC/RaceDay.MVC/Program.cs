
var builder = WebApplication.CreateBuilder(args);

// Add MVC services
builder.Services.AddControllersWithViews();

// Configure communication with RaceDay.API
builder.Services.AddHttpClient("RaceDayAPI", client =>
{
    var apiUrl = builder.Configuration["ApiSettings:BaseUrl"]
        ?? throw new InvalidOperationException(
            "API BaseUrl is not configured.");

    client.BaseAddress = new Uri(apiUrl);
});

// Enable session management
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

// Enable sessions
app.UseSession();

app.UseAuthorization();

// Configure MVC routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
