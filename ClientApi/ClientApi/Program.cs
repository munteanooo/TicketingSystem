using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TicketingSystem.Infrastructure.Persistence;
using TicketingSystem.Infrastructure.Extensions;
using Client.Application.Feature.Tickets.Commands.Create;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Serilog Configuration
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(configuration)
    .CreateLogger();

builder.Host.UseSerilog();

// DbContext - PostgreSQL
var connectionString = configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString, npgsqlOptions =>
    {
        npgsqlOptions.MigrationsAssembly("TicketingSystem.Infrastructure");
        npgsqlOptions.EnableRetryOnFailure(
            maxRetryCount: 3,
            maxRetryDelay: TimeSpan.FromSeconds(5),
            errorCodesToAdd: null
        );
    }));

// Infrastructure Setup
builder.Services.AddInfrastructure(configuration);

// MediatR Configuration
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateTicketCommand).Assembly);
});

// JWT Authentication
var jwtSettings = configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var jwtIssuer = jwtSettings["Issuer"] ?? throw new InvalidOperationException("JWT Issuer not configured");
var jwtAudience = jwtSettings["Audience"] ?? throw new InvalidOperationException("JWT Audience not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
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

builder.Services.AddAuthorization();

// Controllers
builder.Services.AddControllers();

// CORS Configuration
var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? new[] { "http://localhost:3000", "http://localhost:5173" };

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

// HttpContextAccessor for User context in services
builder.Services.AddHttpContextAccessor();

// Build Application
var app = builder.Build();

// Middleware Pipeline
app.UseHttpsRedirection();
app.UseSerilogRequestLogging();
app.UseCors("AllowSpecificOrigins");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Error Handling Middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";
        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;
        Log.Error(exception, "Unhandled exception occurred");
        await context.Response.WriteAsJsonAsync(new
        {
            statusCode = StatusCodes.Status500InternalServerError,
            message = "An internal server error occurred",
            detail = app.Environment.IsDevelopment() ? exception?.Message : null
        });
    });
});

app.Run();