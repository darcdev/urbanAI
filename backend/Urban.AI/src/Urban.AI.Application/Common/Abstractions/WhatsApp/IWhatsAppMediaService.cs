namespace Urban.AI.Application.Common.Abstractions.WhatsApp;

public interface IWhatsAppMediaService
{
    Task<byte[]> DownloadMediaFromUrlAsync(string mediaUrl, CancellationToken cancellationToken = default);
    Task<byte[]> DownloadMediaAsync(string mediaId, CancellationToken cancellationToken = default);
}
