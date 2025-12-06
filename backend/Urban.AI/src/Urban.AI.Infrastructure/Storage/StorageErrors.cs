namespace Urban.AI.Infrastructure.Storage;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Infrastructure.Storage.Resources;
#endregion

internal static class StorageErrors
{
    public static readonly Error FileNotFound = new(
        nameof(StorageResources.FileNotFound),
        StorageResources.FileNotFound);

    public static readonly Error FileUploadFailed = new(
        nameof(StorageResources.FileUploadFailed),
        StorageResources.FileUploadFailed);

    public static readonly Error FileDeleteFailed = new(
        nameof(StorageResources.FileDeleteFailed),
        StorageResources.FileDeleteFailed);

    public static readonly Error FileRetrievalFailed = new(
        nameof(StorageResources.FileRetrievalFailed),
        StorageResources.FileRetrievalFailed);

    public static readonly Error PresignedUrlGenerationFailed = new(
        nameof(StorageResources.PresignedUrlGenerationFailed),
        StorageResources.PresignedUrlGenerationFailed);

    public static readonly Error BucketCreationFailed = new(
        nameof(StorageResources.BucketCreationFailed),
        StorageResources.BucketCreationFailed);

    public static readonly Error InvalidFile = new(
        nameof(StorageResources.InvalidFile),
        StorageResources.InvalidFile);

    public static readonly Error BucketNameRequired = new(
        nameof(StorageResources.BucketNameRequired),
        StorageResources.BucketNameRequired);

    public static readonly Error FilePathOrNameRequired = new(
        nameof(StorageResources.FilePathOrNameRequired),
        StorageResources.FilePathOrNameRequired);

    public static readonly Error FileContentRequired = new(
        nameof(StorageResources.FileContentRequired),
        StorageResources.FileContentRequired);

    public static readonly Error InvalidBase64Content = new(
        nameof(StorageResources.InvalidBase64Content),
        StorageResources.InvalidBase64Content);
}
