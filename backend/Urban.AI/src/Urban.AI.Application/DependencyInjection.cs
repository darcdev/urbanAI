namespace Urban.AI.Application;

#region Usings
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Urban.AI.Application.Common.Abstractions.Behaviors;
using Urban.AI.Application.Geography.Common;
#endregion


public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = typeof(DependencyInjection).Assembly;

        services.AddMediatR(configuration =>
        {
            configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);

            configuration.AddOpenBehavior(typeof(LoggingBehavior<,>));

            configuration.AddOpenBehavior(typeof(ValidationBehavior<,>));
        });

        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly, includeInternalTypes: true);
        services.AddTransient<IGeographyDataParser, GeographyDataParser>();

        return services;
    }
}