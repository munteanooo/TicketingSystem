using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TicketingSystem.API.Middleware;
using TicketingSystem.Application.Contracts;
using TicketingSystem.Domain.Entities;
using TicketingSystem.Infrastructure;
using TicketingSystem.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Servicii din Core/Infrastructure ---
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

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

// --- 3. Autentificare JWT ---
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
        ClockSkew = TimeSpan.Zero // Setați la zero pentru testare precisă
    };
});

// --- 4. CORS (Permitem accesul de la portul Blazor: 7119) ---
builder.Services.AddCors(options =>
{
    options.AddPolicy("BlazorPolicy", policy =>
        policy.WithOrigins("https://localhost:7119")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials());
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 5. SEED DATA & MIGRATIONS ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<TicketingSystemDbContext>();
        await context.Database.MigrateAsync();

        var userManager = services.GetRequiredService<UserManager<User>>();
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
                CreatedAt = DateTime.UtcNow
            };
            await userManager.CreateAsync(newAdmin, "Password123!");
            Console.WriteLine("--> Admin creat: admin@test.com / Password123!");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"--> Eroare Seed: {ex.Message}");
    }
}

// --- 6. Pipeline Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();

// UseCors trebuie să fie ÎNAINTE de Authentication
app.UseCors("BlazorPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();