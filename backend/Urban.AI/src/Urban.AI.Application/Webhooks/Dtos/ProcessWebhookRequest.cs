namespace Urban.AI.Application.Webhooks.Dtos;

public record ProcessWebhookRequest
{
    public string EventType { get; init; } = string.Empty;
    public string IdempotencyKey { get; init; } = string.Empty;
    public bool IsBatch { get; init; }
    public int BatchSize { get; init; }
    public KapsoWebhookPayload Payload { get; init; } = new();
}
