using CoffeeShopApi.Application.Interfaces;
using CoffeeShopApi.Application.Services;
using CoffeeShopApi.Infrastructure.Data;
using CoffeeShopApi.Infrastructure.Repositories;
using CoffeeShopApi.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

namespace CoffeeShopApi.Api.Extensions;

public static class DependencyInjection
{
    public const string CorsPolicy = "DefaultCors";
    public const string AuthRateLimitPolicy = "auth";

    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ProductService>();
        services.AddScoped<UserService>();
        services.AddScoped<AuthService>();
        
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

        services.AddSingleton(new JwtOptions(
            configuration.Required("JWT_ISSUER"),
            configuration.Required("JWT_AUDIENCE"),
            configuration.Required("JWT_SECRET"),
            int.TryParse(configuration["JWT_EXPIRES_MINUTES"], out var minutes) ? minutes : 15,
            int.TryParse(configuration["JWT_REFRESH_TOKEN_DAYS"], out var days) ? days : 7));


        services.AddSingleton<ITokenService, JwtTokenService>();

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<AppDbContext>());

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IPasswordHasher, PasswordHasher>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        
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

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new()
                {
                    ValidIssuer = configuration.Required("JWT_ISSUER"),
                    ValidAudience = configuration.Required("JWT_AUDIENCE"),
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration.Required("JWT_SECRET"))),
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
            });       

        services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy(AuthRateLimitPolicy, httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 5,
                        Window = TimeSpan.FromMinutes(1),
                        QueueLimit = 0
                    }));
        });

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