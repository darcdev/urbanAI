namespace Urban.AI.Domain.Webhooks;

/// <summary>
/// Represents a webhook message received from Kapso (WhatsApp)
/// </summary>
public class KapsoWebhookMessage
{
    #region Constants
    private const string TextMessageType = "text";
    private const string ImageMessageType = "image";
    private const string AudioMessageType = "audio";
    private const string DocumentMessageType = "document";
    #endregion

    public string MessageId { get; private set; } = string.Empty;
    public string From { get; private set; } = string.Empty;
    public string MessageType { get; private set; } = string.Empty;
    public string? TextContent { get; private set; }
    public KapsoMediaInfo? MediaInfo { get; private set; }
    public DateTime ReceivedAt { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public string? IdempotencyKey { get; private set; }

    private KapsoWebhookMessage() { }

    public static KapsoWebhookMessage Create(
        string messageId,
        string from,
        string messageType,
        string eventType,
        string? textContent = null,
        KapsoMediaInfo? mediaInfo = null,
        string? idempotencyKey = null)
    {
        return new KapsoWebhookMessage
        {
            MessageId = messageId,
            From = from,
            MessageType = messageType,
            EventType = eventType,
            TextContent = textContent,
            MediaInfo = mediaInfo,
            ReceivedAt = DateTime.UtcNow,
            IdempotencyKey = idempotencyKey
        };
    }

    public bool IsTextMessage() => MessageType.Equals(TextMessageType, StringComparison.OrdinalIgnoreCase);
    public bool IsImageMessage() => MessageType.Equals(ImageMessageType, StringComparison.OrdinalIgnoreCase);
    public bool IsAudioMessage() => MessageType.Equals(AudioMessageType, StringComparison.OrdinalIgnoreCase);
    public bool IsDocumentMessage() => MessageType.Equals(DocumentMessageType, StringComparison.OrdinalIgnoreCase);
    public bool HasMedia() => MediaInfo is not null;
}
