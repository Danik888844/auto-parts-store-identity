using AutoPartsIdentity.Business.Validators.Users;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AutoPartsIdentity.Business.ServiceRegistrations;

public static class FluentValidationServiceRegistrations
{
    public static IServiceCollection AddFluentValidationServices(this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        #region User

        services.AddValidatorsFromAssemblyContaining<UserCreateCommandValidator>();

        #endregion
        
        return services;
    }
}