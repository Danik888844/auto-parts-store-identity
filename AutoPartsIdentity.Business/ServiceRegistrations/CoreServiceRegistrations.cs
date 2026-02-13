using System.Text;
using AutoPartsIdentity.Business.Services;
using AutoPartsIdentity.Business.Services.Interfaces;
using AutoPartsIdentity.Core.Helpers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Newtonsoft.Json;

namespace AutoPartsIdentity.Business.ServiceRegistrations;

public static class CoreServiceRegistrations
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services, IConfiguration configuration,
        IHostEnvironment environment)
    {
        #region Authentification

        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = "apsAuth";
                    options.DefaultChallengeScheme = "apsAuth";
            })
            .AddJwtBearer("apsAuth", options =>
            {
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = async context =>
                    {
                        context.HandleResponse();
                        context.Response.StatusCode = 401;
                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(JsonConvert.SerializeObject(new
                        {
                            Success = false,
                            Message = "Invalid access token."
                        }));
                    }
                };
                
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(configuration.GetSection("JwtSettings").GetSection("SigningKey")
                            .Value!)),
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidIssuer = configuration.GetSection("JwtSettings").GetSection("Issuer").Value,
                    ValidateAudience = true,
                    ValidAudience = configuration.GetSection("JwtSettings").GetSection("Audience").Value,
                    ValidateLifetime = true,
                    RequireExpirationTime = true,
                    ClockSkew = TimeSpan.Zero
                };
            });
            
        services.AddAuthorization();

        #endregion
        
        services.AddSwaggerGen(s =>
        {
            s.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoPartsIdentity", Version = "v1" });
            s.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "Enter 'Bearer' [space] and then your token in the text input below.'",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            
            s.AddSecurityRequirement(document => new OpenApiSecurityRequirement
            {
                [new OpenApiSecuritySchemeReference("Bearer", document)] = []
            });
        });
        
        services.AddControllers();

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
        
        services.Configure<JwtTokenOptions>(configuration.GetSection("JwtSettings"));
        services.AddSingleton<ITokenCacheService, TokenCacheService>();
        services.AddHttpContextAccessor();
        
        return services;
    }
}