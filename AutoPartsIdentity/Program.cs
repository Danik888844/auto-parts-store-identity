using AutoPartsIdentity.Business;
using AutoPartsIdentity.Business.ServiceRegistrations;
using AutoPartsIdentity.DataAccess.Contexts;
using AutoPartsIdentity.DataAccess.Enums;
using AutoPartsIdentity.DataAccess.Models.DatabaseModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables("APP_");

#region Service Registration

AppConfig.Configuration = builder.Configuration;
builder.Services.AddServices(builder.Configuration, builder.Environment);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

#endregion

builder.Services.AddHttpClient();

var app = builder.Build();
var logger = app.Services.GetRequiredService<ILogger<Program>>();

#region Auto Migrate
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDb>();

    var appliedMigrations = dbContext.Database.GetAppliedMigrations();
    var pendingMigrations = dbContext.Database.GetPendingMigrations();
    var missingMigrations = pendingMigrations.Except(appliedMigrations).ToList();

    if (missingMigrations.Any())
    {
        logger.LogInformation("There are pending migrations:");
        foreach (var migration in missingMigrations)
        {
            logger.LogInformation("{Migration}", migration);
        }
    }
    else
    {
        logger.LogInformation("Migrations have been successfully applied.");
    }

    try
    {
        dbContext.Database.Migrate();

        logger.LogInformation("Database was successfully migrated.");
    }
    catch (Exception ex)
    {
        logger.LogError("An error occurred while migrating the database. {Message}", ex.Message);
        logger.LogError("{StackTrace}", ex.StackTrace);
    }
}
#endregion

#region Base Role and User creating

await using (var scope = app.Services.CreateAsyncScope())
{
    logger.LogInformation("Check admin user...");
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

    var roleNames = new[]
    {
        UserRoleEnum.Administrator,
        UserRoleEnum.Seller
    };
    
    const string adminRole = UserRoleEnum.Administrator;
    var adminUserName = "admin";
    var adminEmail = "admin@mail.com";
    var adminPassword = "QWE123qwe";

    // create a roles
    foreach (var roleName in roleNames)
    {
        if (await roleManager.RoleExistsAsync(roleName))
            continue;

        var create = await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
        if (!create.Succeeded)
        {
            logger.LogError("Failed to create role: {Role}. Errors: {Errors}",
                roleName,
                string.Join("; ", create.Errors.Select(e => e.Description)));
        }
        else
        {
            logger.LogInformation("Role created: {Role}", roleName);
        }
    }

    // create a user
    var admin = await userManager.FindByEmailAsync(adminEmail);
    if (admin is null)
    {
        admin = new User
        {
            FirstName = "Administrator",
            LastName = "",
            UserName = adminUserName,
            Email = adminEmail,
            EmailConfirmed = true,
            IsActive = true
        };

        var userCreate = await userManager.CreateAsync(admin, adminPassword);
        if (!userCreate.Succeeded)
        {
            logger.LogError("Failed to create user: {User}", adminEmail);
            logger.LogError("{Error}", string.Join("; ", userCreate.Errors.Select(e => e.Description)));
        }
        else
        {
            logger.LogInformation("User created: {User}", adminEmail);
        }
    }

    // set role for user
    if (!await userManager.IsInRoleAsync(admin, adminRole))
    {
        var addRole = await userManager.AddToRoleAsync(admin, adminRole);
        if (!addRole.Succeeded){
            logger.LogError("Failed to set role {Role} to user {User}", adminRole, adminEmail);
            logger.LogError("{Error}", string.Join("; ", addRole.Errors.Select(e => e.Description)));
        }
        else
        {
            logger.LogInformation("Role {Role} related with user {User}", adminRole, adminEmail);
        }
    }
}

#endregion

// Enable/Disable Swagger
if (Convert.ToBoolean(builder.Configuration.GetSection("IsSwaggerEnabled").Value!))
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Lax,
    Secure = CookieSecurePolicy.SameAsRequest,
});

app.UseCors(builder.Configuration.GetSection("CorsLabel").Value!);
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();