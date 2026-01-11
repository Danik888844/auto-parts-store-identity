using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;

namespace AutoPartsIdentity.Business.ServiceRegistrations;

public static class CoreServiceRegistrations
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        services.AddControllers();
        services.AddAuthorization();

        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo{ Title = "AutoPartsIdentity", Version = "v1" });
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter 'Bearer' [space] and then your token in the text input below.'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
        });

        #region Cors

        services.AddCors(options =>
        {
            options.AddPolicy(name: configuration.GetSection("CorsLabel").Value!,
                builder =>
                {
                    builder.WithMethods(
                        configuration.GetSection("Methods").GetChildren().Select(i => i.Value).ToArray()!);
                    builder.AllowAnyHeader();
                    builder.AllowCredentials();
                    builder.WithOrigins(
                        configuration.GetSection("Origins").Value?.Split(',').Select(i => i.Trim()).ToArray()!);
                    builder.Build();
                }
            );
        });

        #endregion
        
        return services;
    }
}