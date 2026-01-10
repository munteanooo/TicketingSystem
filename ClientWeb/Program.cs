using Blazored.LocalStorage;
using ClientWeb.Services;
using ClientWeb.Components.Pages;
using ClientWeb.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using ClientWeb.Components;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddRazorComponents()
       .AddInteractiveServerComponents();

// HTTP Client Factory cu BaseAddress
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Injectare simplă HttpClient
builder.Services.AddScoped(sp =>
{
    return new HttpClient
    {
        BaseAddress = new Uri(builder.Configuration["ApiBaseUrl"] ?? "https://localhost:5001")
    };
});

// Authentication & Authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = IdentityConstants.ApplicationScheme;
    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
})
.AddIdentityCookies();

builder.Services.AddAuthorization();

// Local Storage
builder.Services.AddBlazoredLocalStorage();

// Register services
builder.Services.AddScoped<IHttpApiService, HttpApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles(); // înlocuiește MapStaticAssets pentru fișiere statice
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseAntiforgery();
// Map Razor Components
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();