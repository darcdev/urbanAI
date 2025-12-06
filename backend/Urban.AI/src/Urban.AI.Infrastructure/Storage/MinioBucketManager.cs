namespace Urban.AI.Infrastructure.Storage;

#region Usings
using Microsoft.Extensions.Logging;
using Urban.AI.Domain.Common.Abstractions;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
#endregion

internal sealed class MinioBucketManager
{
    private readonly IMinioClient _minioClient;
    private readonly ILogger _logger;

    public MinioBucketManager(IMinioClient minioClient, ILogger logger)
    {
        _minioClient = minioClient;
        _logger = logger;
    }

    public async Task<Result> EnsureBucketExistsAsync(string bucketName, CancellationToken cancellationToken = default)
    {
        try
        {
            var existsArgs = new BucketExistsArgs()
                .WithBucket(bucketName);

            var exists = await _minioClient.BucketExistsAsync(existsArgs, cancellationToken);

            if (exists)
            {
                return Result.Success();
            }

            var makeBucketArgs = new MakeBucketArgs()
                .WithBucket(bucketName);

            await _minioClient.MakeBucketAsync(makeBucketArgs, cancellationToken);

            _logger.LogInformation("Bucket created successfully in MinIO: {BucketName}", bucketName);

            return Result.Success();
        }
        catch (MinioException ex)
        {
            _logger.LogError(ex, "MinIO error while ensuring bucket exists: {BucketName}", bucketName);
            return Result.Failure(StorageErrors.BucketCreationFailed);
        }
    }
}
