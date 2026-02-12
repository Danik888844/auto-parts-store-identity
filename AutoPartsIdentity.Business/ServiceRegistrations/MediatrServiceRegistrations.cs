using AutoPartsIdentity.Business.Cqrs.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoPartsIdentity.Business.ServiceRegistrations;

public static class MediatrServiceRegistrations
{
    public static IServiceCollection AddMediatrServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        #region User

        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(UserCreateCommand.Handler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(UserLoginCommand.Handler).Assembly));
        services.AddMediatR(cnf =>
            cnf.RegisterServicesFromAssemblies(typeof(UserGetListCommand.Handler).Assembly));

        #endregion
        
        return services;
    }
}