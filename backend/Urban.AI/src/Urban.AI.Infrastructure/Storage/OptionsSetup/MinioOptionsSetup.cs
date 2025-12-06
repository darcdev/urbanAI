namespace Urban.AI.Infrastructure.Storage.OptionsSetup;

#region MyRegion
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
#endregion

public class MinioOptionsSetup(IConfiguration configuration) : IConfigureOptions<MinioOptions>
{
    #region Constants
    private const string SectionName = "Minio";
    #endregion

    private readonly IConfiguration _configuration = configuration;

    public void Configure(MinioOptions options)
    {
        _configuration.GetSection(SectionName).Bind(options);
    }
}
