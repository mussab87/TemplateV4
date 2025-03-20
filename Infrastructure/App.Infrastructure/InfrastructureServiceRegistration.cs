using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace App.Infrastructure { }

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddEntityFrameworkSqlServer();
        services.AddDbContext<AppDbContext>(
                    options => options.UseSqlServer(configuration.GetConnectionString("ServiceDBConnection"))
                    .LogTo(Console.WriteLine, LogLevel.Information));



        services.AddIdentity<User, Role>(options =>
        {
            var appSettings = configuration.GetSection("AppSetting");

            options.Password.RequireDigit = appSettings.GetValue("PassRequireDigit", true);
            options.Password.RequireLowercase = appSettings.GetValue("PassRequireLowercase", true);
            options.Password.RequireUppercase = appSettings.GetValue("PassRequireUppercase", true);
            options.Password.RequireNonAlphanumeric = appSettings.GetValue("PassRequireNonAlphanumeric", true);
            options.Password.RequiredLength = appSettings.GetValue("UserPasswordLength", 6);

            // Lockout settings (optional)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;

            // User settings (optional)
            options.User.RequireUniqueEmail = true;
        })
    .AddRoleManager<RoleManager<Role>>()
    .AddSignInManager<SignInManager<User>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

        services.ConfigureApplicationCookie(options =>
        {
            options.ExpireTimeSpan = TimeSpan.FromMinutes(Convert.ToInt32(configuration.GetSection("AppSetting").GetValue("UserSessionTimeOut", 30)));
            options.LoginPath = "/Account/Login";
            options.SlidingExpiration = true;
        });

        services.Configure<SecurityStampValidatorOptions>(options =>
        {
            options.ValidationInterval = TimeSpan.Zero;
        });


        services.AddTransient<IDbInitializer, DbInitializer>();

        return services;
    }
}

