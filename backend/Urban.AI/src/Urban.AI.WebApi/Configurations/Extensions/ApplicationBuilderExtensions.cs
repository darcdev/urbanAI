namespace Urban.AI.WebApi.Configurations.Extensions;

#region Usings
using Microsoft.EntityFrameworkCore;
using Urban.AI.Infrastructure.Database;
using Urban.AI.WebApi.Configurations.Middlewares;
#endregion

public static class ApplicationBuilderExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();

        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }

    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}