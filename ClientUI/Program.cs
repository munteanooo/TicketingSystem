using Blazored.LocalStorage;
using ClientUI.Components;
using ClientUI.Services;
using ClientUI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Blazor Server (Razor Components)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 1. AUTHENTICATION - PENTRU [Authorize]
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/login";
        options.LogoutPath = "/logout";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    });

// 2. AUTHORIZATION
builder.Services.AddAuthorization();

// HttpClient către API
builder.Services.AddHttpClient<IAuthService, AuthService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["ApiSettings:BaseUrl"] ?? "http://localhost:5053";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient<ITicketService, TicketService>((sp, client) =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var baseUrl = config["ApiSettings:BaseUrl"] ?? "http://localhost:5053";
    client.BaseAddress = new Uri(baseUrl);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// LocalStorage
builder.Services.AddBlazoredLocalStorage();

var app = builder.Build();

// Configure pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseAntiforgery();

app.UseAuthentication();     
app.UseAuthorization();     

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
