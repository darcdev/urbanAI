namespace Urban.AI.Infrastructure.Storage;

#region Usings
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
using Urban.AI.Infrastructure.Storage.OptionsSetup;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
#endregion

internal sealed class MinioStorageService : IStorageService
{
    #region Constants
    private const string DefaultContentType = "application/octet-stream";
    #endregion

    #region Private Members
    private readonly IMinioClient _minioClient;
    private readonly ILogger<MinioStorageService> _logger;
    private readonly MinioOptions _options;
    private readonly MinioBucketManager _bucketManager;
    #endregion

    public MinioStorageService(
        IMinioClient minioClient,
        ILogger<MinioStorageService> logger,
        IOptions<MinioOptions> options)
    {
        _minioClient = minioClient;
        _logger = logger;
        _options = options.Value;
        _bucketManager = new MinioBucketManager(minioClient, logger);
    }

    public async Task<Result<byte[]>> GetFile(File file, CancellationToken cancellationToken = default)
    {
        var validationResult = MinioFileValidator.ValidateForGet(file);
        if (validationResult.IsFailure) return Result.Failure<byte[]>(validationResult.Error);

        var objectKey = BuildObjectKey(file);

        try
        {
            using var memoryStream = new MemoryStream();

            var getObjectArgs = new GetObjectArgs()
                .WithBucket(file.NameBucket)
                .WithObject(objectKey)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(getObjectArgs, cancellationToken);

            _logger.LogInformation(
                "Object retrieved successfully from MinIO. Bucket: {Bucket}, Key: {Key}, Size: {Size} bytes",
                file.NameBucket,
                objectKey,
                memoryStream.Length);

            return Result.Success(memoryStream.ToArray());
        }
        catch (ObjectNotFoundException)
        {
            _logger.LogWarning(
                "Object not found in MinIO. Bucket: {Bucket}, Key: {Key}",
                file.NameBucket,
                objectKey);

            return Result.Failure<byte[]>(StorageErrors.FileNotFound);
        }
        catch (MinioException ex)
        {
            _logger.LogError(
                ex,
                "MinIO error while retrieving object. Bucket: {Bucket}, Key: {Key}",
                file.NameBucket,
                objectKey);

            return Result.Failure<byte[]>(StorageErrors.FileRetrievalFailed);
        }
    }

    public async Task<Result<string>> SaveFile(File file, CancellationToken cancellationToken = default)
    {
        var validationResult = MinioFileValidator.ValidateForSave(file);
        if (validationResult.IsFailure) return Result.Failure<string>(validationResult.Error);

        var objectKey = BuildObjectKey(file);

        try
        {
            if (_options.AutoCreateBuckets)
            {
                var bucketResult = await _bucketManager.EnsureBucketExistsAsync(file.NameBucket, cancellationToken);
                if (bucketResult.IsFailure) return Result.Failure<string>(bucketResult.Error);
            }

            var base64Content = MinioPathHelper.ExtractBase64Payload(file.Content);
            var fileBytes = Convert.FromBase64String(base64Content);

            using var memoryStream = new MemoryStream(fileBytes);

            var contentType = string.IsNullOrWhiteSpace(file.Mimetype)
                ? DefaultContentType
                : file.Mimetype;

            var putObjectArgs = new PutObjectArgs()
                .WithBucket(file.NameBucket)
                .WithObject(objectKey)
                .WithStreamData(memoryStream)
                .WithObjectSize(memoryStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putObjectArgs, cancellationToken);

            _logger.LogInformation(
                "Object saved successfully to MinIO. Bucket: {Bucket}, Key: {Key}, Size: {Size} bytes, ContentType: {ContentType}",
                file.NameBucket,
                objectKey,
                memoryStream.Length,
                contentType);

            return Result.Success(objectKey);
        }
        catch (MinioException ex)
        {
            _logger.LogError(
                ex,
                "MinIO error while saving object. Bucket: {Bucket}, Key: {Key}",
                file.NameBucket,
                objectKey);

            return Result.Failure<string>(StorageErrors.FileUploadFailed);
        }
    }

    public async Task<Result> DeleteFile(File file, CancellationToken cancellationToken = default)
    {
        var validationResult = MinioFileValidator.ValidateForDelete(file);
        if (validationResult.IsFailure) return validationResult;

        var objectKey = BuildObjectKey(file);

        try
        {
            var removeObjectArgs = new RemoveObjectArgs()
                .WithBucket(file.NameBucket)
                .WithObject(objectKey);

            await _minioClient.RemoveObjectAsync(removeObjectArgs, cancellationToken);

            _logger.LogInformation(
                "Object deleted successfully from MinIO. Bucket: {Bucket}, Key: {Key}",
                file.NameBucket,
                objectKey);

            return Result.Success();
        }
        catch (ObjectNotFoundException)
        {
            _logger.LogWarning(
                "Attempted to delete non-existent object. Bucket: {Bucket}, Key: {Key}",
                file.NameBucket,
                objectKey);

            return Result.Success();
        }
        catch (MinioException ex)
        {
            _logger.LogError(
                ex,
                "MinIO error while deleting object. Bucket: {Bucket}, Key: {Key}",
                file.NameBucket,
                objectKey);

            return Result.Failure(StorageErrors.FileDeleteFailed);
        }
    }

    public async Task<Result<string>> GetPresignedUrl(
        File file,
        int expiryInHours = 24,
        CancellationToken cancellationToken = default)
    {
        var validationResult = MinioFileValidator.ValidateForGet(file);
        if (validationResult.IsFailure) return Result.Failure<string>(validationResult.Error);

        var objectKey = BuildObjectKey(file);
        var expiryInSeconds = expiryInHours > 0
            ? expiryInHours * 3600
            : _options.PresignedUrlExpiryInHours * 3600;

        try
        {
            var presignedGetObjectArgs = new PresignedGetObjectArgs()
                .WithBucket(file.NameBucket)
                .WithObject(objectKey)
                .WithExpiry(expiryInSeconds);

            var presignedUrl = await _minioClient.PresignedGetObjectAsync(presignedGetObjectArgs);

            _logger.LogInformation(
                "Pre-signed URL generated successfully. Bucket: {Bucket}, Key: {Key}, ExpiryHours: {ExpiryHours}",
                file.NameBucket,
                objectKey,
                expiryInHours);

            return Result.Success(presignedUrl);
        }
        catch (MinioException ex)
        {
            _logger.LogError(
                ex,
                "MinIO error while generating pre-signed URL. Bucket: {Bucket}, Key: {Key}",
                file.NameBucket,
                objectKey);

            return Result.Failure<string>(StorageErrors.PresignedUrlGenerationFailed);
        }
    }

    #region Private Methods
    private static string BuildObjectKey(File file)
    {
        return string.IsNullOrWhiteSpace(file.Filename)
            ? MinioPathHelper.NormalizePath(file.Path)
            : MinioPathHelper.BuildObjectKey(file.Path, file.Filename);
    }
    #endregion
}