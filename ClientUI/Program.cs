using Blazored.LocalStorage;
using ClientUI.Components;
using ClientUI.Services;
using ClientUI.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Razor Components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

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

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

// static files + antiforgery sunt ok
app.MapStaticAssets();
app.UseAntiforgery();

// Blazor Server
app.MapRazorComponents<App>()
   .AddInteractiveServerRenderMode();

app.Run();
