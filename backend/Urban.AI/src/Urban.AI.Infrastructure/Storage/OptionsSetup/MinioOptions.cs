namespace Urban.AI.Infrastructure.Storage.OptionsSetup;

public sealed class MinioOptions
{
    public string Host { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool IsSecureSSL { get; init; }
    public bool AutoCreateBuckets { get; init; } = true;
    public int PresignedUrlExpiryInHours { get; init; } = 24;
}
