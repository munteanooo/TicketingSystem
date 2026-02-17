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

// --- 1. Servicii de bază ---
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

// --- 4. CORS (Configurare Militară) ---
builder.Services.AddCors(options =>
{
     options.AddPolicy("BlazorPolicy", policy =>
     {
          policy.WithOrigins(
                  "https://localhost:5001",
                  "http://localhost:5000",
                  "https://ticketingsystem-ene4cdd9atdzdtd3.westeurope-01.azurewebsites.net" // URL Frontend
              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials()
              .SetIsOriginAllowedToAllowWildcardSubdomains();
     });
});

// --- 5. Alte servicii ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 6. Middleware (ORDINEA ESTE CRITICĂ) ---

// Activează Swagger și în producție momentan pentru a verifica dacă API-ul răspunde
app.UseSwagger();
app.UseSwaggerUI(c =>
{
     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketing API V1");
     c.RoutePrefix = "swagger"; // API-ul va fi la /swagger
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Servirea fișierelor Blazor
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

// CORS trebuie să fie exact aici
app.UseCors("BlazorPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Fallback pentru a asigura că Refresh-ul în browser nu dă 404
app.MapFallbackToFile("index.html");

app.Run();