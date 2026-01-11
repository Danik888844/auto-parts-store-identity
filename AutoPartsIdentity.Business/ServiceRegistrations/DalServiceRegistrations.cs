using AutoPartsIdentity.DataAccess.Contexts;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoPartsIdentity.Business.ServiceRegistrations;

public static class DalServiceRegistrations
{
    public static IServiceCollection AddDalServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        #region DbContext

        services.AddDbContext<IdentityDb>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString("IdentityDatabase"),
                builder => builder.MigrationsAssembly("AutoPartsIdentity")
            );
        });

        services.AddDataProtection();
        services
            .AddIdentityCore<User>(opt =>
            {
                opt.User.RequireUniqueEmail = true;

                // требования пароля
                opt.Password.RequiredLength = 6;
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequireUppercase = false;
                opt.Password.RequireLowercase = false;
                opt.Password.RequireDigit = false;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<IdentityDb>()
            .AddSignInManager()
            .AddDefaultTokenProviders();
        
        #endregion

        #region Dals
        
        //services.AddScoped<IUserDal, UserDal>();
        
        #endregion

        return services;
    }
}