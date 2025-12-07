namespace Urban.AI.Infrastructure.Email;

public sealed class EmailOptions
{
    public string SmtpServer { get; init; } = string.Empty;
    public int SmtpPort { get; init; }
    public string SenderEmail { get; init; } = string.Empty;
    public string SenderName { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public bool EnableSsl { get; init; } = true;
}
