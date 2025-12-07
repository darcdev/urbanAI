namespace Urban.AI.Application.Webhooks.Dtos;

public record KapsoWebhookPayload
{
    public KapsoMessage Message { get; init; } = new();
    public KapsoConversation? Conversation { get; init; }
}

public record KapsoMessage
{
    public string From { get; init; } = string.Empty;
    public string Id { get; init; } = string.Empty;
    public string Timestamp { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public KapsoText? Text { get; init; }
    public KapsoImage? Image { get; init; }
    public KapsoLocation? Location { get; init; }
    public KapsoAudio? Audio { get; init; }
    public KapsoDocument? Document { get; init; }
}

public record KapsoText
{
    public string Body { get; init; } = string.Empty;
}

public record KapsoImage
{
    public string Id { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string Sha256 { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public string Link { get; init; } = string.Empty;
    public string? Caption { get; init; }
}

public record KapsoLocation
{
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string? Name { get; init; }
    public string? Address { get; init; }
}

public record KapsoAudio
{
    public string Id { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
}

public record KapsoDocument
{
    public string Id { get; init; } = string.Empty;
    public string Url { get; init; } = string.Empty;
    public string MimeType { get; init; } = string.Empty;
    public string? Filename { get; init; }
}

public record KapsoConversation
{
    public string Id { get; init; } = string.Empty;
    public string ContactName { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
}
