using System.Text;
using Client.Application.Contracts.Messaging;
using Client.Application.Contracts.Persistence;
using Client.Application.Contracts.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TicketingSystem.Infrastructure.Identity;
using TicketingSystem.Infrastructure.Persistence;
using TicketingSystem.Infrastructure.Persistence.Repositories;
using TicketingSystem.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// 1. Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Client.Application.Feature.Tickets.Commands.Create.CreateTicketCommand).Assembly);
});

// 2. Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// 3. Swagger Configuration with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Ticketing System API",
        Version = "v1",
        Description = "API for Ticketing System with JWT Authentication"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});


// Repositories
builder.Services.AddScoped<ITicketRepository, TicketRepository>();
builder.Services.AddScoped<ITicketMessageRepository, TicketMessageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// 4. PostgreSQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

// 5. Identity configuration + parola relaxed
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 4;
})
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

// 6. Register Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUser, CurrentUser>();

// 7. JWT Authentication Configuration
var jwtSettings = configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

if (jwtKey.Length < 32)
    throw new InvalidOperationException("JWT Key must be at least 32 characters long");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        ValidateIssuer = true,
        ValidIssuer = jwtIssuer,
        ValidateAudience = true,
        ValidAudience = jwtAudience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// 8. Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireClientRole", policy => policy.RequireRole("Client"));
    options.AddPolicy("RequireTechSupportRole", policy => policy.RequireRole("TechSupport"));
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("TechSupport"));
    options.AddPolicy("RequireAuthenticated", policy => policy.RequireAuthenticatedUser());
});

// 9. CORS
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:5173", "https://localhost:5001", "http://localhost:5000" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// 10. HealthChecks
builder.Services.AddHealthChecks();

var app = builder.Build();

// ========== Middleware ==========
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketing System API v1"));
}

app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Seed roles
await SeedRoles(app);

// Error handling
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        Log.Error(exception, "Unhandled exception at {Path}", exceptionHandlerPathFeature?.Path);

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = StatusCodes.Status500InternalServerError,
            message = "Internal server error",
            detail = app.Environment.IsDevelopment() ? exception?.Message : null,
            path = exceptionHandlerPathFeature?.Path,
            timestamp = DateTime.UtcNow
        });
    });
});

// Root endpoint
app.MapGet("/", () => Results.Json(new
{
    message = "Ticketing System API",
    version = "1.0",
    documentation = "/swagger",
    health = "/health",
    endpoints = new
    {
        auth = "/api/auth",
        tickets = "/api/tickets",
        users = "/api/users"
    }
}));

Log.Information("Starting Ticketing System API...");
app.Run();

static async Task SeedRoles(WebApplication application)
{
    try
    {
        using var scope = application.Services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

        if (!await roleManager.RoleExistsAsync("Client"))
            await roleManager.CreateAsync(new IdentityRole<Guid>("Client"));

        if (!await roleManager.RoleExistsAsync("TechSupport"))
            await roleManager.CreateAsync(new IdentityRole<Guid>("TechSupport"));

        Log.Information("Roles seeded successfully");
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Error seeding roles");
    }
}
