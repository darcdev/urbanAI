namespace Urban.AI.WebApi.Configurations.OpenApi;

public static class ApplicationBuilderExtensions
{
    private const string SwaggerEndpoint = $"/swagger/{{0}}/swagger.json";

    public static IApplicationBuilder UseSwaggerWithUi(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            var descriptions = app.DescribeApiVersions();

            foreach (var description in descriptions)
            {
                var url = string.Format(SwaggerEndpoint, description.GroupName);
                var name = description.GroupName.ToUpperInvariant();

                options.SwaggerEndpoint(url, name);
            }
        });

        return app;
    }
}