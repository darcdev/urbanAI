#region Usings
using Asp.Versioning;
using Asp.Versioning.Builder;
using Urban.AI.Application;
using Urban.AI.Infrastructure;
using Urban.AI.Infrastructure.Storage.OptionsSetup;
using Urban.AI.WebApi.Configurations.Extensions;
using Urban.AI.WebApi.Configurations.OpenApi;
#endregion

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOptions<MinioOptionsSetup>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddPresentation();

builder.Services.AddAuthorization();

var app = builder.Build();

ApiVersionSet apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    .ReportApiVersions()
    .Build();

RouteGroupBuilder versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwaggerWithUi();
    app.ApplyMigrations();

    // REMARK: Uncomment if you want to seed initial data.
    // app.SeedData();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseCustomExceptionHandler();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
