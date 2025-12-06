namespace Urban.AI.WebApi.Configurations.Extensions;

#region Usings
using Urban.AI.Application.Geography.SeedGeographyData;
using MediatR;
#endregion

internal static class SeedDataExtensions
{
    internal static void SeedGeographyData(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        var sender = scope.ServiceProvider.GetRequiredService<ISender>();

        var command = new SeedGeographyDataCommand();

        sender.Send(command).GetAwaiter().GetResult();
    }
}