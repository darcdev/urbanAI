namespace Urban.AI.Application.Common.Abstractions.Storage;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
#endregion

public interface IStorageService
{
    Task<Result<string>> SaveFile(File file, CancellationToken cancellationToken = default);

    Task<Result<byte[]>> GetFile(File file, CancellationToken cancellationToken = default);
    
    Task<Result> DeleteFile(File file, CancellationToken cancellationToken = default);

    Task<Result<string>> GetPresignedUrl(File file, int expiryInHours = 24, CancellationToken cancellationToken = default);
}
