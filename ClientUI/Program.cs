using ClientUI.Services;
using ClientUI.Services.Interfaces;
using ClientUI.Components;
using Blazored.LocalStorage;
using MudBlazor.Services;
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Razor Components
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// 2️⃣ MudBlazor
builder.Services.AddMudServices();

// 3️⃣ Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// 4️⃣ Authentication & Authorization
builder.Services.AddAuthorization();
builder.Services.AddAuthorizationCore();
builder.Services.AddAntiforgery();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(
    sp => sp.GetRequiredService<JwtAuthStateProvider>());

// 5️⃣ HTTP Client Handler pentru JWT
builder.Services.AddScoped<AuthHttpClientHandler>();

// 6️⃣ AuthService - cu AuthHttpClientHandler
builder.Services.AddHttpClient<IAuthService, AuthService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5053");
})
.AddHttpMessageHandler<AuthHttpClientHandler>();

// 7️⃣ TicketService - cu AuthHttpClientHandler
builder.Services.AddHttpClient<ITicketService, TicketService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5053");
})
.AddHttpMessageHandler<AuthHttpClientHandler>();

var app = builder.Build();

// 8️⃣ Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAntiforgery();

// 9️⃣ Map Razor Components
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .DisableAntiforgery();

app.Run();