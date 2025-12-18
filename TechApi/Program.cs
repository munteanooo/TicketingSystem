using Microsoft.EntityFrameworkCore;
using TicketingSystem.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// --- DbContext ---
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// --- Controllers ---
builder.Services.AddControllers();

// --- Swagger / OpenAPI ---
builder.Services.AddEndpointsApiExplorer(); // pentru Minimal APIs
builder.Services.AddSwaggerGen();

// Build app
var app = builder.Build();

// --- Middleware ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
