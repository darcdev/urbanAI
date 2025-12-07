namespace Urban.AI.Infrastructure.Email;

#region Usings
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Urban.AI.Application.Common.Abstractions.Email;
#endregion

internal sealed class EmailService : IEmailService
{
    #region Constants
    private const string LeaderCredentialsSubject = "Welcome to UrbanAI - Leader Access Credentials";
    private const string LeaderCredentialsBodyTemplate = @"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2>Welcome to UrbanAI, {0} {1}!</h2>
            <p>You have been assigned as a leader in the UrbanAI system.</p>
            <p>Your login credentials are:</p>
            <ul>
                <li><strong>Email:</strong> {2}</li>
                <li><strong>Temporary Password:</strong> {3}</li>
            </ul>
            <p>Please login to the system and change your password as soon as possible.</p>
            <p><strong>Important:</strong> Keep this information secure and do not share it with anyone.</p>
            <br/>
            <p>Best regards,</p>
            <p>The UrbanAI Team</p>
        </body>
        </html>";

    private const string OrganizationCredentialsSubject = "Welcome to UrbanAI - Organization Access Credentials";
    private const string OrganizationCredentialsBodyTemplate = @"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2>Welcome to UrbanAI, {0} {1}!</h2>
            <p>Your organization <strong>{4}</strong> has been registered in the UrbanAI system.</p>
            <p>Your login credentials are:</p>
            <ul>
                <li><strong>Email:</strong> {2}</li>
                <li><strong>Temporary Password:</strong> {3}</li>
            </ul>
            <p>As an organization user, you will have access to statistical reports and analytics.</p>
            <p>Please login to the system and change your password as soon as possible.</p>
            <p><strong>Important:</strong> Keep this information secure and do not share it with anyone.</p>
            <br/>
            <p>Best regards,</p>
            <p>The UrbanAI Team</p>
        </body>
        </html>";
    #endregion

    private readonly EmailOptions _emailOptions;

    public EmailService(IOptions<EmailOptions> emailOptions)
    {
        _emailOptions = emailOptions.Value;
    }

    public async Task<bool> SendEmailAsync(
        string toEmail,
        string subject,
        string body,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var message = new MailMessage
            {
                From = new MailAddress(_emailOptions.SenderEmail, _emailOptions.SenderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);

            using var smtpClient = new SmtpClient(_emailOptions.SmtpServer, _emailOptions.SmtpPort)
            {
                Credentials = new NetworkCredential(_emailOptions.Username, _emailOptions.Password),
                EnableSsl = _emailOptions.EnableSsl
            };

            await smtpClient.SendMailAsync(message, cancellationToken);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<bool> SendLeaderCredentialsEmailAsync(
        string toEmail,
        string firstName,
        string lastName,
        string email,
        string temporaryPassword,
        CancellationToken cancellationToken = default)
    {
        var body = string.Format(LeaderCredentialsBodyTemplate, firstName, lastName, email, temporaryPassword);
        return await SendEmailAsync(toEmail, LeaderCredentialsSubject, body, cancellationToken);
    }

    public async Task<bool> SendOrganizationCredentialsEmailAsync(
        string toEmail,
        string firstName,
        string lastName,
        string email,
        string organizationName,
        string temporaryPassword,
        CancellationToken cancellationToken = default)
    {
        var body = string.Format(OrganizationCredentialsBodyTemplate, firstName, lastName, email, temporaryPassword, organizationName);
        return await SendEmailAsync(toEmail, OrganizationCredentialsSubject, body, cancellationToken);
    }
}
