namespace Urban.AI.Application.Webhooks.Dtos;

public record ProcessWebhookResponse
{
    public bool Success { get; init; }
    public int ProcessedMessages { get; init; }
    public List<string> ProcessedMessageIds { get; init; } = new();
    public string? ErrorMessage { get; init; }
}
