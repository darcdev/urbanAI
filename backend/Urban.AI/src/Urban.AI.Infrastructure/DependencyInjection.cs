namespace Urban.AI.Infrastructure;

#region Usings
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Minio;
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.Caching;
using Urban.AI.Application.Common.Abstractions.Clock;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
using Urban.AI.Infrastructure.Auth.Authentication;
using Urban.AI.Infrastructure.Auth.Authorization;
using Urban.AI.Infrastructure.Caching;
using Urban.AI.Infrastructure.Clock;
using Urban.AI.Infrastructure.Database;
using Urban.AI.Infrastructure.Database.Config;
using Urban.AI.Infrastructure.Database.Repositories.User;
using Urban.AI.Infrastructure.Storage;
using Urban.AI.Infrastructure.Storage.OptionsSetup;
using AuthenticationOptions = Auth.Authentication.AuthenticationOptions;
using AuthenticationService = Auth.Authentication.AuthenticationService;
using IAuthenticationService = Application.Common.Abstractions.Authentication.IAuthenticationService;
#endregion

public static class DependencyInjection
{
    #region Constants
    private const string DbConnectionString = "DefaultConnectionDb";
    private const string RedisConnectionString = "RedisConnection";
    private const string MinioOptionsSectionName = "Minio";
    private const string AuthenticationOptionsSectionName = "Authentication";
    private const string KeycloakOptionsSectionName = "Keycloak";
    #endregion

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        AddDatabase(services, configuration);
        AddRepositories(services);
        AddAuthentication(services, configuration);
        AddAuthorization(services);
        AddCaching(services, configuration);
        AddStorage(services, configuration);

        return services;
    }

    #region Private Methods
    private static void AddDatabase(
        IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DbConnectionString) ??
                               throw new ArgumentNullException(nameof(configuration));


        var dbProvider = new ConnectionString(connectionString).GetDatabaseImplemented();

        if (dbProvider is DatabaseProvider.PostgreSQL) AddPostgreSql(services, connectionString);

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());
    }

    private static void AddRepositories(IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }

    private static void AddAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.Configure<AuthenticationOptions>(configuration.GetSection(AuthenticationOptionsSectionName));
        services.ConfigureOptions<JwtBearerOptionsSetup>();
        services.Configure<KeycloakOptions>(configuration.GetSection(KeycloakOptionsSectionName));
        services.AddTransient<AdminAuthorizationDelegatingHandler>();
        services.AddHttpClient<IAuthenticationService, AuthenticationService>((serviceProvider, httpClient) =>
        {
            KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        })
        .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IIdentityProvider, KeycloakService>((serviceProvider, httpClient) =>
        {
            KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;
            httpClient.BaseAddress = new Uri(keycloakOptions.AdminUrl);
        })
        .AddHttpMessageHandler<AdminAuthorizationDelegatingHandler>();

        services.AddHttpClient<IJwtService, JwtService>((serviceProvider, httpClient) =>
        {
            KeycloakOptions keycloakOptions = serviceProvider.GetRequiredService<IOptions<KeycloakOptions>>().Value;

            httpClient.BaseAddress = new Uri(keycloakOptions.TokenUrl);
        });

        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
    }

    private static void AddAuthorization(IServiceCollection services)
    {
        services.AddScoped<AuthorizationService>();
        services.AddTransient<IClaimsTransformation, CustomClaimsTransformation>();
        services.AddTransient<IAuthorizationHandler, PermissionAuthorizationHandler>();
        services.AddTransient<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
    }

    private static void AddCaching(IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(RedisConnectionString) ??
                                  throw new ArgumentNullException(nameof(configuration));

        services.AddStackExchangeRedisCache(options => options.Configuration = connectionString);

        services.AddSingleton<ICacheService, CacheService>();
    }

    private static void AddStorage(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MinioOptions>(configuration.GetSection(MinioOptionsSectionName));

        var minioOptions = configuration.GetSection(MinioOptionsSectionName).Get<MinioOptions>()
            ?? throw new ArgumentNullException(nameof(configuration));

        services.AddMinio(configureClient => configureClient
            .WithEndpoint(minioOptions.Host)
            .WithCredentials(minioOptions.Username, minioOptions.Password)
            .WithSSL(minioOptions.IsSecureSSL)
            .Build());

        services.AddScoped<IStorageService, MinioStorageService>();
    }
    #endregion

    #region Extra settings
    private static void AddPostgreSql(IServiceCollection services, string connection)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connection)
                   .UseSnakeCaseNamingConvention());
    }

    private static DatabaseProvider GetDatabaseImplemented(this ConnectionString connectionString)
    {
        if (connectionString.Value.Contains("Server=", StringComparison.OrdinalIgnoreCase) &&
            connectionString.Value.Contains("Database=", StringComparison.OrdinalIgnoreCase) &&
            connectionString.Value.Contains("User ID=", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseProvider.SqlServer;
        }

        if (connectionString.Value.Contains("Host=", StringComparison.OrdinalIgnoreCase) &&
            connectionString.Value.Contains("Port=", StringComparison.OrdinalIgnoreCase) &&
            connectionString.Value.Contains("Database=", StringComparison.OrdinalIgnoreCase) &&
            connectionString.Value.Contains("Username=", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseProvider.PostgreSQL;
        }

        if (connectionString.Value.Contains("mongodb://", StringComparison.OrdinalIgnoreCase))
        {
            return DatabaseProvider.MongoDB;
        }

        throw new NotSupportedException("The provided connection string does not match any supported database provider.");
    }
    #endregion
}
