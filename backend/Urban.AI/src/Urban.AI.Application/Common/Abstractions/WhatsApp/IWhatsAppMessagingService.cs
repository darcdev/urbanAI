using Urban.AI.Domain.Common.Abstractions;

namespace Urban.AI.Application.Common.Abstractions.WhatsApp;

public interface IWhatsAppMessagingService
{
    Task<Result> SendTextMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
