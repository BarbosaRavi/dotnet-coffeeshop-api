using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Application.Services;
using CoffeeShopApi.Infrastructure.Data;
using CoffeeShopApi.Infrastructure.Repositories;
using CoffeeShopApi.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace CoffeeShopApi.Api.Extensions;

public static class DependencyInjection
{
    public const string CorsPolicy = "DefaultCors";

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        services.AddScoped<UserService>();
        
        return services;
    }

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = new NpgsqlConnectionStringBuilder
        {
            Host     = configuration.Required("DB_HOST"),
            Port     = int.Parse(configuration["DB_PORT"] ?? "5432"),
            Database = configuration.Required("DB_DATABASE"),
            Username = configuration.Required("DB_USERNAME"),
            Password = configuration.Required("DB_PASSWORD")
        }.ConnectionString;

        services.AddDbContext<AppDbContext>(options => options
            .UseNpgsql(connectionString)
            .UseSnakeCaseNamingConvention()
        );

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        
        return services;

    }

    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = (configuration["CORS_ALLOWED_ORIGINS"] ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(options => options.AddPolicy(CorsPolicy, policy => policy
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));

        return services;
    }
}

public static class ConfigurationExtensions
{
    public static string Required(this IConfiguration configuration, string key) =>
        configuration[key]
        ?? throw new InvalidOperationException(
            $"Missing required configuration value '{key}'. Check your .env file.");
}