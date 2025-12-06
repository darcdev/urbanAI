namespace Urban.AI.Infrastructure.Storage;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
#endregion

internal sealed class MinioFileValidator
{
    public static Result ValidateForGet(File file)
    {
        if (file is null)
        {
            return Result.Failure(StorageErrors.InvalidFile);
        }

        if (string.IsNullOrWhiteSpace(file.NameBucket))
        {
            return Result.Failure(StorageErrors.BucketNameRequired);
        }

        if (string.IsNullOrWhiteSpace(file.Path) && string.IsNullOrWhiteSpace(file.Filename))
        {
            return Result.Failure(StorageErrors.FilePathOrNameRequired);
        }

        return Result.Success();
    }

    public static Result ValidateForSave(File file)
    {
        if (file is null)
        {
            return Result.Failure(StorageErrors.InvalidFile);
        }

        if (string.IsNullOrWhiteSpace(file.NameBucket))
        {
            return Result.Failure(StorageErrors.BucketNameRequired);
        }

        if (string.IsNullOrWhiteSpace(file.Path) && string.IsNullOrWhiteSpace(file.Filename))
        {
            return Result.Failure(StorageErrors.FilePathOrNameRequired);
        }

        if (string.IsNullOrWhiteSpace(file.Content))
        {
            return Result.Failure(StorageErrors.FileContentRequired);
        }

        if (!IsValidBase64(file.Content))
        {
            return Result.Failure(StorageErrors.InvalidBase64Content);
        }

        return Result.Success();
    }

    public static Result ValidateForDelete(File file)
    {
        if (file is null)
        {
            return Result.Failure(StorageErrors.InvalidFile);
        }

        if (string.IsNullOrWhiteSpace(file.NameBucket))
        {
            return Result.Failure(StorageErrors.BucketNameRequired);
        }

        if (string.IsNullOrWhiteSpace(file.Path) && string.IsNullOrWhiteSpace(file.Filename))
        {
            return Result.Failure(StorageErrors.FilePathOrNameRequired);
        }

        return Result.Success();
    }

    private static bool IsValidBase64(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        var base64Content = MinioPathHelper.ExtractBase64Payload(content);

        try
        {
            Convert.FromBase64String(base64Content);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
