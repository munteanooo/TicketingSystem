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

// --- 2. Configurare HTTP & API ---
// Am setat direct URL-ul de Azure pentru a evita eroarea 404/Connection Refused
var apiBaseAddress = "https://ticketingsystem-ene4cdd9atdzdtd3.westeurope-01.azurewebsites.net/";

builder.Services.AddTransient<JwtInterceptor>();

builder.Services.AddHttpClient("TicketingAPI", client =>
{
     client.BaseAddress = new Uri(apiBaseAddress);
})
.AddHttpMessageHandler<JwtInterceptor>();

// Înregistrăm HttpClient-ul principal care va fi folosit de servicii
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("TicketingAPI"));

// --- 3. Autentificare & Stare ---
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// --- 4. Servicii de Business ---
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();

await builder.Build().RunAsync();