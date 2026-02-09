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

// --- 1. Servicii de Bază ---
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

// --- 2. Autentificare JWT ---
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

// --- 3. Configurare CORS (MODIFICATĂ) ---
builder.Services.AddCors(options =>
{
     options.AddPolicy("BlazorPolicy", policy =>
         policy.WithOrigins(
                 "https://localhost:7119",
                 "https://ticketingsystem-ene4cdd9atdzdtd3.westeurope-01.azurewebsites.net"
               )
               .AllowAnyMethod()
               .AllowAnyHeader()
               .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 4. Seed Data & Migrations ---
using (var scope = app.Services.CreateScope())
{
     var services = scope.ServiceProvider;
     try
     {
          var context = services.GetRequiredService<TicketingSystemDbContext>();
          await context.Database.MigrateAsync();

          var userManager = services.GetRequiredService<UserManager<User>>();
          var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

          string[] roleNames = { "Admin", "TechSupport", "Client" };
          foreach (var roleName in roleNames)
          {
               if (!await roleManager.RoleExistsAsync(roleName))
               {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
               }
          }

          var adminEmail = "admin@test.com";
          var adminUser = await userManager.FindByEmailAsync(adminEmail);

          if (adminUser == null)
          {
               var newAdmin = new User
               {
                    Id = Guid.NewGuid(),
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Admin System",
                    Role = "Admin",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    EmailConfirmed = true
               };
               var result = await userManager.CreateAsync(newAdmin, "Password123!");
               if (result.Succeeded)
               {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
               }
          }
     }
     catch (Exception ex)
     {
          Console.WriteLine($"--> Eroare Seed: {ex.Message}");
     }
}

// --- 5. Pipeline HTTP (ORDINE CRITICĂ) ---
if (app.Environment.IsDevelopment())
{
     app.UseSwagger();
     app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// CORS trebuie să fie după HttpsRedirection și înainte de Routing/Auth
app.UseCors("BlazorPolicy");

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();