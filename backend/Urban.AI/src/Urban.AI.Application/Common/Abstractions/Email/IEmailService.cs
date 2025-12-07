namespace Urban.AI.Application.Common.Abstractions.Email;

public interface IEmailService
{
    Task<bool> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default);

    Task<bool> SendLeaderCredentialsEmailAsync(
        string toEmail,
        string firstName,
        string lastName,
        string email,
        string temporaryPassword,
        CancellationToken cancellationToken = default);
}
