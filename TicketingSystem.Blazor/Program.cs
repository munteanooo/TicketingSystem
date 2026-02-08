using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using TicketingSystem.Blazor;
using TicketingSystem.Blazor.Services;
using TicketingSystem.Blazor.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- 1. UI & Utilitare ---
builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();

// --- 2. Infrastructură HTTP & Securitate ---
// IMPORTANT: Înregistrează interceptorul ca Transient
builder.Services.AddTransient<JwtInterceptor>();

builder.Services.AddHttpClient("TicketingAPI", client =>
{
    // Portul 7176 trebuie să corespundă cu launchSettings.json din API (https)
    client.BaseAddress = new Uri("https://localhost:7176/");
})
.AddHttpMessageHandler<JwtInterceptor>();

// Înregistrează HttpClient-ul implicit pentru a fi folosit în servicii
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("TicketingAPI"));

// --- 3. Autentificare & Stare ---
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// --- 4. Servicii de Business ---
builder.Services.AddScoped<AuthService>(); // Asigură-te că AuthService folosește HttpClient-ul de mai sus
builder.Services.AddScoped<ITicketService, TicketService>();

await builder.Build().RunAsync();