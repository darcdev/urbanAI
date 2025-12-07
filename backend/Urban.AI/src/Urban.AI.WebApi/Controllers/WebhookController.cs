using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;
using Urban.AI.Application.Webhooks.Dtos;
using Urban.AI.Application.Webhooks.ProcessWebhook;
namespace Urban.AI.WebApi.Controllers;

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/webhook")]
public class WebhookController : ControllerBase
{
    #region Constants
    private const string SignatureHeader = "X-Webhook-Signature";
    private const string EventTypeHeader = "X-Webhook-Event";
    private const string IdempotencyKeyHeader = "X-Idempotency-Key";
    private const string BatchHeader = "X-Webhook-Batch";
    private const string BatchSizeHeader = "X-Batch-Size";
    #endregion

    #region Private Members
    private readonly ISender sender;
    private readonly IConfiguration configuration;
    private readonly ILogger<WebhookController> logger;
    #endregion

    public WebhookController(
        ISender sender,
        IConfiguration configuration,
        ILogger<WebhookController> logger)
    {
        this.sender = sender;
        this.configuration = configuration;
        this.logger = logger;
    }

    /// <summary>
    /// Endpoint to receive webhooks from Kapso (WhatsApp)
    /// </summary>
    /// <param name="payload">The webhook payload from Kapso</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Always returns 200 OK for idempotency</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ReceiveWebhook(
        CancellationToken cancellationToken)
    {
        var signature = Request.Headers[SignatureHeader].FirstOrDefault();
        var eventType = Request.Headers[EventTypeHeader].FirstOrDefault() ?? string.Empty;
        var idempotencyKey = Request.Headers[IdempotencyKeyHeader].FirstOrDefault();
        var batchHeader = Request.Headers[BatchHeader].FirstOrDefault();
        var batchSizeHeader = Request.Headers[BatchSizeHeader].FirstOrDefault();

        var isBatch = bool.TryParse(batchHeader, out var batch) && batch;
        var batchSize = int.TryParse(batchSizeHeader, out var size) ? size : 0;

        logger.LogInformation(
            "Webhook received. EventType: {EventType}, IdempotencyKey: {IdempotencyKey}, IsBatch: {IsBatch}, BatchSize: {BatchSize}",
            eventType,
            idempotencyKey,
            isBatch,
            batchSize);

        Request.EnableBuffering();

        using var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true);
        var rawBody = await reader.ReadToEndAsync(cancellationToken);
        Request.Body.Position = 0;

        logger.LogInformation("Raw webhook payload (first 1000 chars): {Payload}", 
            rawBody.Length > 1000 ? rawBody.Substring(0, 1000) + "..." : rawBody);

        if (!string.IsNullOrWhiteSpace(signature) && !ValidateSignature(rawBody, signature))
        {
            logger.LogWarning("Invalid webhook signature received. Signature: {Signature}. Proceeding anyway for testing.", signature);
        }

        KapsoWebhookPayload? payload;
        try
        {
            payload = System.Text.Json.JsonSerializer.Deserialize<KapsoWebhookPayload>(
                rawBody, 
                new System.Text.Json.JsonSerializerOptions 
                { 
                    PropertyNameCaseInsensitive = true 
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to deserialize webhook payload");
            return BadRequest(new { error = "Invalid payload format" });
        }

        if (payload == null)
        {
            logger.LogError("Payload deserialization resulted in null");
            return BadRequest(new { error = "Empty payload" });
        }

        var request = new ProcessWebhookRequest
        {
            EventType = eventType,
            IdempotencyKey = idempotencyKey ?? string.Empty,
            IsBatch = isBatch,
            BatchSize = batchSize,
            Payload = payload
        };

        var command = new ProcessWebhookCommand(request);
        var result = await sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            logger.LogError("Failed to process webhook. Error: {Error}", result.Error.Name);
        }

        return Ok(new
        {
            success = true,
            message = "Webhook received and processing",
            processedMessages = result.IsSuccess ? result.Value.ProcessedMessages : 0
        });
    }

    private bool ValidateSignature(string payload, string? receivedSignature)
    {
        if (string.IsNullOrWhiteSpace(receivedSignature))
        {
            logger.LogWarning("No signature provided in webhook request");
            return false;
        }

        var secret = configuration["Kapso:Secret"];
        if (string.IsNullOrWhiteSpace(secret))
        {
            logger.LogError("Kapso:Secret is not configured");
            return false;
        }

        var computedSignature = ComputeHmacSha256(payload, secret);
        var isValid = string.Equals(computedSignature, receivedSignature, StringComparison.OrdinalIgnoreCase);

        if (!isValid)
        {
            logger.LogWarning(
                "Signature mismatch. Expected: {Expected}, Received: {Received}",
                computedSignature,
                receivedSignature);
        }

        return isValid;
    }

    private static string ComputeHmacSha256(string payload, string secret)
    {
        var keyBytes = Encoding.UTF8.GetBytes(secret);
        var payloadBytes = Encoding.UTF8.GetBytes(payload);

        using var hmac = new HMACSHA256(keyBytes);
        var hashBytes = hmac.ComputeHash(payloadBytes);

        return Convert.ToHexString(hashBytes).ToLowerInvariant();
    }
}
