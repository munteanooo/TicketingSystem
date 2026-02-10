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

// --- 2. JWT ---
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

// --- 3. CORS (Adaugat "*" pentru testare rapida) ---
builder.Services.AddCors(options =>
{
     options.AddPolicy("BlazorPolicy", policy =>
         policy.AllowAnyOrigin() // Temporar, pana rezolvam conexiunea
               .AllowAnyMethod()
               .AllowAnyHeader());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. Pipeline HTTP (ORDINEA ESTE CRITICĂ) ---

// Forțăm Swagger să fie disponibil oriunde
app.UseSwagger(c => {
     c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

app.UseSwaggerUI(c =>
{
     c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketing API V1");
     c.RoutePrefix = string.Empty; // Swagger va fi DIRECT la adresa de baza (fara /swagger)
});

app.UseMiddleware<ExceptionHandlingMiddleware>();

// Dacă primești 405 constant pe Azure, uneori HttpsRedirection face probleme cu Proxy-ul Azure
// app.UseHttpsRedirection(); 

app.UseRouting();

app.UseCors("BlazorPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Adaugă o rută de test directă pentru a vedea dacă aplicația trăiește
app.MapGet("/test-live", () => "API-ul este ONLINE!");

app.Run();