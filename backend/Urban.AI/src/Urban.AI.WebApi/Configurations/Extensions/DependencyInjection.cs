namespace Urban.AI.WebApi.Configurations.Extensions;

#region Usings
using Asp.Versioning;
using Microsoft.OpenApi.Models;
#endregion

public static class DependencyInjection
{
    #region Constants
    private const string SchemaName = "Bearer";
    private const string GroupNameFormat = "'v'V";
    private const string NameSecurityAuthorization = "Authorization";
    private const string DescriptionSecurity = "Please enter into field the word 'Bearer' followed by a space and the JWT value";
    #endregion

    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services.AddProblemDetails();

        services.AddControllers();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Define the Security Scheme for JWT
            options.AddSecurityDefinition(SchemaName, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = DescriptionSecurity,
                Name = NameSecurityAuthorization,
                Type = SecuritySchemeType.ApiKey,
                Scheme = SchemaName
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = SchemaName
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });


        AddApiVersioning(services);

        return services;
    }


    private static void AddApiVersioning(IServiceCollection services)
    {
        services
            .AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddMvc()
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = GroupNameFormat;
                options.SubstituteApiVersionInUrl = true;
            });
    }
}