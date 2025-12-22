//using Microsoft.AspNetCore.Identity;
//using TicketingSystem.Infrastructure.Data;
//using TicketingSystem.Infrastructure.Identity;
//using Microsoft.EntityFrameworkCore;

//var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
//    .AddEntityFrameworkStores<ApplicationIdentityDbContext>()
//    .AddDefaultTokenProviders();

//builder.Services.AddControllers();
//builder.Services.AddOpenApi();

//var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
//    app.MapOpenApi();
//}

//app.UseHttpsRedirection();

//app.UseAuthentication(); 
//app.UseAuthorization();

//app.MapControllers();

//app.Run();
