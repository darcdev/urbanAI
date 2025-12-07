using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using Urban.AI.Application.Common.Abstractions.WhatsApp;

namespace Urban.AI.Infrastructure.WhatsApp;

internal sealed class WhatsAppMediaService : IWhatsAppMediaService
{
    #region Constants
    private const string MediaEndpointTemplate = "media/{0}";
    private const string AuthorizationScheme = "Bearer";
    #endregion

    #region Private Members
    private readonly HttpClient httpClient;
    private readonly ILogger<WhatsAppMediaService> logger;
    private readonly string baseUrl;
    private readonly string apiKey;
    #endregion

    public WhatsAppMediaService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WhatsAppMediaService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        baseUrl = configuration["Kapso:BaseUrl"] ?? throw new InvalidOperationException("Kapso:BaseUrl is not configured");
        apiKey = configuration["Kapso:ApiKey"] ?? throw new InvalidOperationException("Kapso:ApiKey is not configured");

        ConfigureHttpClient();
    }

    public async Task<byte[]> DownloadMediaFromUrlAsync(string mediaUrl, CancellationToken cancellationToken = default)
    {
        try
        {
            logger.LogInformation("Downloading media from URL. Url: {Url}", mediaUrl);

            using var request = new HttpRequestMessage(HttpMethod.Get, mediaUrl);
            
            if (!mediaUrl.Contains("cloudflarestorage.com", StringComparison.OrdinalIgnoreCase))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, apiKey);
            }

            var response = await httpClient.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Failed to download media from URL. Url: {Url}, StatusCode: {StatusCode}, Error: {Error}",
                    mediaUrl,
                    response.StatusCode,
                    errorContent);

                throw new HttpRequestException(
                    $"Failed to download media. Status: {response.StatusCode}, Url: {mediaUrl}");
            }

            var mediaBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            logger.LogInformation(
                "Successfully downloaded media from URL. Url: {Url}, Size: {Size} bytes",
                mediaUrl,
                mediaBytes.Length);

            return mediaBytes;
        }
        catch (Exception ex) when (ex is not HttpRequestException)
        {
            logger.LogError(ex, "Error downloading media from URL. Url: {Url}", mediaUrl);
            throw;
        }
    }

    public async Task<byte[]> DownloadMediaAsync(string mediaId, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(MediaEndpointTemplate, mediaId);
            var requestUri = new Uri(new Uri(baseUrl), endpoint);

            logger.LogInformation("Downloading media from Kapso. MediaId: {MediaId}", mediaId);

            var response = await httpClient.GetAsync(requestUri, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Failed to download media from Kapso. MediaId: {MediaId}, StatusCode: {StatusCode}, Error: {Error}",
                    mediaId,
                    response.StatusCode,
                    errorContent);

                throw new HttpRequestException(
                    $"Failed to download media. Status: {response.StatusCode}, MediaId: {mediaId}");
            }

            var mediaBytes = await response.Content.ReadAsByteArrayAsync(cancellationToken);

            logger.LogInformation(
                "Successfully downloaded media from Kapso. MediaId: {MediaId}, Size: {Size} bytes",
                mediaId,
                mediaBytes.Length);

            return mediaBytes;
        }
        catch (Exception ex) when (ex is not HttpRequestException)
        {
            logger.LogError(ex, "Error downloading media from Kapso. MediaId: {MediaId}", mediaId);
            throw;
        }
    }

    private void ConfigureHttpClient()
    {
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationScheme, apiKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("*/*"));
    }
}
