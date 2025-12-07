#region Usings
using System.Globalization;
using Asp.Versioning;
using Asp.Versioning.Builder;
using CommunityToolkit.HighPerformance;
using Urban.AI.Application;
using Urban.AI.Infrastructure;
using Urban.AI.Infrastructure.Storage.OptionsSetup;
using Urban.AI.WebApi.Configurations.Extensions;
using Urban.AI.WebApi.Configurations.OpenApi;
#endregion

CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<MinioOptionsSetup>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin() // Permite cualquier origen
                          .AllowAnyMethod() // Permite cualquier método (GET, POST, etc.)
                          .AllowAnyHeader()); // Permite cualquier encabezado
});


var app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

// Configure the HTTP request pipeline.
app.UseSwaggerWithUi();

app.ApplyMigrations();

app.MapGet("/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.UseCors("AllowAll");

if (!app.Environment.IsProduction())
{
    app.UseHttpsRedirection();
}

app.UseExceptionHandler();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
