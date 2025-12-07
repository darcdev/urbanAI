using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Urban.AI.Application.Common.Abstractions.WhatsApp;
using Urban.AI.Domain.Common;
using Urban.AI.Domain.Common.Abstractions;

namespace Urban.AI.Infrastructure.WhatsApp;

internal sealed class WhatsAppMessagingService : IWhatsAppMessagingService 
{
    #region Constants
    private const string SendMessageEndpointTemplate = "meta/whatsapp/v24.0/{0}/messages";
    private const string ApiKeyHeader = "X-API-Key";
    #endregion

    #region Private Members
    private readonly HttpClient httpClient;
    private readonly ILogger<WhatsAppMessagingService> logger;
    private readonly string baseUrl;
    private readonly string apiKey;
    private readonly string phoneNumberId;
    #endregion

    public WhatsAppMessagingService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<WhatsAppMessagingService> logger)
    {
        this.httpClient = httpClient;
        this.logger = logger;
        baseUrl = configuration["Kapso:BaseUrl"] ?? throw new InvalidOperationException("Kapso:BaseUrl is not configured");
        apiKey = configuration["Kapso:ApiKey"] ?? throw new InvalidOperationException("Kapso:ApiKey is not configured");
        phoneNumberId = configuration["Kapso:PhoneNumberId"] ?? throw new InvalidOperationException("Kapso:PhoneNumberId is not configured");

        ConfigureHttpClient();
    }

    public async Task<Result> SendTextMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            var endpoint = string.Format(SendMessageEndpointTemplate, phoneNumberId);
            var requestUri = new Uri(new Uri(baseUrl), endpoint);

            var payload = new
            {
                messaging_product = "whatsapp",
                to = phoneNumber,
                type = "text",
                text = new
                {
                    body = message
                }
            };

            var jsonContent = JsonSerializer.Serialize(payload);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            logger.LogInformation(
                "Sending WhatsApp message to {PhoneNumber}. Message: {Message}, Endpoint: {Endpoint}",
                phoneNumber,
                message,
                endpoint);

            var response = await httpClient.PostAsync(requestUri, content, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                logger.LogError(
                    "Failed to send WhatsApp message. PhoneNumber: {PhoneNumber}, StatusCode: {StatusCode}, Error: {Error}",
                    phoneNumber,
                    response.StatusCode,
                    errorContent);

                return Result.Failure(new Error(
                    "WhatsApp.SendMessageFailed",
                    $"Failed to send WhatsApp message. Status: {response.StatusCode}"));
            }

            logger.LogInformation(
                "WhatsApp message sent successfully. PhoneNumber: {PhoneNumber}",
                phoneNumber);

            return Result.Success();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error sending WhatsApp message. PhoneNumber: {PhoneNumber}", phoneNumber);
            return Result.Failure(new Error(
                "WhatsApp.SendMessageError",
                "An error occurred while sending WhatsApp message"));
        }
    }

    private void ConfigureHttpClient()
    {
        httpClient.DefaultRequestHeaders.Add(ApiKeyHeader, apiKey);
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }
}
