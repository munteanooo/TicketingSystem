// using ClientWeb.Components;
// <<<<<<< feature/navigation-flow
// =======
// using ClientWeb.Components.Account;
// using ClientWeb.Data;
// using Microsoft.AspNetCore.Components.Authorization;
// using Microsoft.AspNetCore.Identity;
// using Microsoft.EntityFrameworkCore;
// using Blazored.LocalStorage;
// >>>>>>> master

// namespace ClientWeb
// {
//     public class Program
//     {
//         public static void Main(string[] args)
//         {
//             var builder = WebApplication.CreateBuilder(args);

//             builder.Services.AddRazorComponents()
//                 .AddInteractiveServerComponents();

//             builder.Services.AddHttpClient();

// <<<<<<< feature/navigation-flow
//             builder.Services.AddAuthentication();
//             builder.Services.AddAuthorization();
// =======
//             builder.Services.AddAuthentication(options =>
//                 {
//                     options.DefaultScheme = IdentityConstants.ApplicationScheme;
//                     options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
//                 })
//                 .AddIdentityCookies();

//             var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//             builder.Services.AddDbContext<ApplicationDbContext>(options =>
//                 options.UseSqlServer(connectionString));
//             builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//             builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
//                 .AddEntityFrameworkStores<ApplicationDbContext>()
//                 .AddSignInManager()
//                 .AddDefaultTokenProviders();

//             builder.Services.AddBlazoredLocalStorage();

//             builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();
// >>>>>>> master

//             var app = builder.Build();

//             app.UseHttpsRedirection();
//             app.UseAntiforgery();

//             app.MapStaticAssets();
//             app.MapRazorComponents<App>()
//                 .AddInteractiveServerRenderMode();

//             app.Run();
//         }
//     }
// }
