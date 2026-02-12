using AutoMapper;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using AutoPartsIdentity.DataAccess.Models.DtoModels.User;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;

namespace AutoPartsIdentity.Business.ServiceRegistrations;

public static class AutoMapServiceRegistrations
{
    public static IServiceCollection AddAutoMapServiceRegistrations(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddSingleton(new MapperConfiguration(config =>
        {
            config.CreateMap<User, UserDto>().ReverseMap();
        },
        NullLoggerFactory.Instance).CreateMapper());

        return services;
    }
}