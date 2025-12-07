namespace Urban.AI.Domain.Webhooks;

/// <summary>
/// Represents media information from a Kapso webhook message
/// </summary>
public class KapsoMediaInfo
{
    public string MediaId { get; private set; } = string.Empty;
    public string MimeType { get; private set; } = string.Empty;
    public string? Sha256 { get; private set; }
    public string? Caption { get; private set; }

    private KapsoMediaInfo() { }

    public static KapsoMediaInfo Create(
        string mediaId,
        string mimeType,
        string? sha256 = null,
        string? caption = null)
    {
        return new KapsoMediaInfo
        {
            MediaId = mediaId,
            MimeType = mimeType,
            Sha256 = sha256,
            Caption = caption
        };
    }

    public bool IsImage() => MimeType.StartsWith("image/", StringComparison.OrdinalIgnoreCase);
    public bool IsAudio() => MimeType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase);
    public bool IsDocument() => MimeType.StartsWith("application/", StringComparison.OrdinalIgnoreCase);
}
