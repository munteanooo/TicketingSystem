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
// Verifică dacă în appsettings.json adresa este corectă pentru Azure
var apiBaseAddress = builder.Configuration["ApiSettings:BaseAddress"]
                     ?? "https://ticketingsystem-api-ene4cdd9atdzd3.westeurope-01.azurewebsites.net/";

// IMPORTANT: Unii browseri/servere sunt sensibili la slash-ul de final în apeluri API
if (!apiBaseAddress.EndsWith("/")) apiBaseAddress += "/";

builder.Services.AddTransient<JwtInterceptor>();

// Înregistrare HttpClient (Named Client)
builder.Services.AddHttpClient("TicketingAPI", client =>
{
     client.BaseAddress = new Uri(apiBaseAddress);
})
.AddHttpMessageHandler<JwtInterceptor>();

// Această linie este CRUCIALĂ: asigură că atunci când injectezi HttpClient simplu în servicii, 
// acesta va folosi setările de mai sus (BaseAddress + Interceptor)
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("TicketingAPI"));

// --- 3. Autentificare & Stare ---
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// --- 4. Servicii de Business ---
// Asigură-te că AuthService și TicketService primesc un HttpClient în constructor
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<ITicketService, TicketService>();

await builder.Build().RunAsync();