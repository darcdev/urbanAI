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
using Urban.AI.Application.Common.Abstractions.Clock;
using Urban.AI.Application.Common.Abstractions.Email;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Organizations;
using Urban.AI.Domain.Geography;
using Urban.AI.Domain.Leaders;
using Urban.AI.Domain.Users;
using Urban.AI.Infrastructure.Auth.Authentication;
using Urban.AI.Infrastructure.Auth.Authorization;
using Urban.AI.Infrastructure.Clock;
using Urban.AI.Infrastructure.Database;
using Urban.AI.Infrastructure.Database.Config;
using Urban.AI.Infrastructure.Database.Repositories.OrganizationUser;
using Urban.AI.Infrastructure.Database.Repositories.Geography;
using Urban.AI.Infrastructure.Database.Repositories.Leader;
using Urban.AI.Infrastructure.Database.Repositories.User;
using Urban.AI.Infrastructure.Email;
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
    private const string EmailOptionsSectionName = "Email";
    #endregion

    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddTransient<IDateTimeProvider, DateTimeProvider>();

        AddDatabase(services, configuration);
        AddRepositories(services);
        AddGeographyRepositories(services);
        AddAuthentication(services, configuration);
        AddAuthorization(services);
        AddCaching(services, configuration);
        AddStorage(services, configuration);
        AddEmail(services, configuration);

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
        services.AddScoped<ILeaderRepository, LeaderRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
    }

    private static void AddGeographyRepositories(IServiceCollection services)
    {
        services.AddScoped<IDepartmentRepository, DepartmentRepository>();
        services.AddScoped<IMunicipalityRepository, MunicipalityRepository>();
        services.AddScoped<ITownshipRepository, TownshipRepository>();
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

    private static void AddEmail(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<EmailOptions>(configuration.GetSection(EmailOptionsSectionName));
        services.AddScoped<IEmailService, EmailService>();
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
