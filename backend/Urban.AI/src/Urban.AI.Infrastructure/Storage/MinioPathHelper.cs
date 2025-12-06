namespace Urban.AI.Infrastructure.Storage;

internal sealed class MinioPathHelper
{
    #region Constants
    private const string ForwardSlash = "/";
    private const string BackSlash = "\\";
    private const string DoubleForwardSlash = "//";
    private const string Base64Prefix = "base64,";
    #endregion

    public static string BuildObjectKey(string path, string filename)
    {
        var normalizedPath = NormalizePath(path);
        
        if (string.IsNullOrWhiteSpace(filename))
        {
            return normalizedPath;
        }

        var normalizedFilename = filename.Trim().Replace(BackSlash, ForwardSlash);

        return string.IsNullOrWhiteSpace(normalizedPath)
            ? normalizedFilename
            : $"{normalizedPath}{ForwardSlash}{normalizedFilename}".Replace(DoubleForwardSlash, ForwardSlash);
    }

    public static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return string.Empty;
        }

        return path
            .Replace(BackSlash, ForwardSlash)
            .Trim()
            .Trim('/')
            .Replace(DoubleForwardSlash, ForwardSlash);
    }

    public static string ExtractBase64Payload(string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return string.Empty;
        }

        var index = content.IndexOf(Base64Prefix, StringComparison.OrdinalIgnoreCase);
        
        return index >= 0 
            ? content[(index + Base64Prefix.Length)..] 
            : content;
    }
}
