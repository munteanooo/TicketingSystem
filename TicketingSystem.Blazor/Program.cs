using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services; // Adăugat pentru MudBlazor
using TicketingSystem.Blazor;
using TicketingSystem.Blazor.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// --- 1. Stocare Locală ---
builder.Services.AddBlazoredLocalStorage();

// --- 2. MudBlazor Services ---
// Această linie înregistrează DialogService, Snackbar, etc.
builder.Services.AddMudServices();

// --- 3. Autentificare & Autorizare ---
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ApiAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp =>
    sp.GetRequiredService<ApiAuthenticationStateProvider>());

// --- 4. Servicii Infastructură HTTP ---
builder.Services.AddScoped<JwtInterceptor>();

// Configurare Client HTTP principal cu Interceptor
builder.Services.AddHttpClient("TicketingAPI", client =>
{
    client.BaseAddress = new Uri("https://localhost:7176/");
})
.AddHttpMessageHandler<JwtInterceptor>();

// Înregistrăm HttpClient-ul implicit pentru a fi injectat simplu cu @inject HttpClient
builder.Services.AddScoped(sp =>
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("TicketingAPI"));

// --- 5. Servicii de Business ---
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<TicketService>();

await builder.Build().RunAsync();