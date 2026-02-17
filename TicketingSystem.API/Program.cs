using System.Text;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicketingSystem.API.Middleware;
using TicketingSystem.Application.Contracts;
using TicketingSystem.Application.Interfaces;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Infrastructure;
using TicketingSystem.Infrastructure.Persistence;
using TicketingSystem.Infrastructure.MediatR;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Servicii ---
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddScoped<IApplicationDbContext>(provider =>
    provider.GetRequiredService<TicketingSystemDbContext>());

builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnitOfWorkBehavior<,>));

// --- 2. Identity ---
builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
     options.Password.RequireDigit = false;
     options.Password.RequiredLength = 6;
     options.Password.RequireNonAlphanumeric = false;
     options.Password.RequireUppercase = false;
     options.Password.RequireLowercase = false;
})
.AddEntityFrameworkStores<TicketingSystemDbContext>()
.AddDefaultTokenProviders();

// --- 3. JWT ---
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"] ?? "O_Cheie_Super_Secreta_Si_Lunga_De_32_Caractere";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
     options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
     options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
     options.TokenValidationParameters = new TokenValidationParameters
     {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidIssuer = jwtSettings["Issuer"],
          ValidAudience = jwtSettings["Audience"],
          ClockSkew = TimeSpan.Zero
     };
});

// --- 4. CORS (CORECTAT) ---
builder.Services.AddCors(options =>
{
     options.AddPolicy("BlazorPolicy", policy =>
     {
          policy.WithOrigins(
                  "https://localhost:5001", // Portul local Blazor
                  "http://localhost:5000",
                  "https://ticketingsystem-ene4cdd9atdzdtd3.westeurope-01.azurewebsites.net" // URL-UL TĂU DE FRONTEND (nu API)
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Adăugat pentru siguranță dacă folosești cookies/Identity
     });
});

// --- 5. Alte servicii ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 6. Middleware ---
// IMPORTANT: Ordinea contează enorm în ASP.NET Core!

if (app.Environment.IsDevelopment())
{
     app.UseSwagger();
     app.UseSwaggerUI(c =>
     {
          c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketing API V1");
     });
}

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Fișierele statice și framework-ul Blazor trebuie să fie înainte de Routing
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// CORS trebuie să fie după UseRouting dar înainte de Authentication/Authorization
app.UseCors("BlazorPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback pentru Blazor WASM
app.MapFallbackToFile("index.html");

app.Run();