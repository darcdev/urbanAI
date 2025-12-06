namespace Urban.AI.Application.Common.Extensions;

internal static class MediaFileExtensions
{
    internal static bool IsValidImageFormat(this byte[] imageBytes)
    {
        // Check for common image file signatures
        if (imageBytes.Length < 4) return false;

        // JPEG
        if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8) return true;

        // PNG
        if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47) return true;

        // WebP
        if (imageBytes.Length >= 12 &&
            imageBytes[0] == 0x52 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46 && imageBytes[3] == 0x46 &&
            imageBytes[8] == 0x57 && imageBytes[9] == 0x45 && imageBytes[10] == 0x42 && imageBytes[11] == 0x50) return true;

        return false;
    }

    internal static string GetContentType(this string fileExtension)
    {
        return fileExtension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".bmp" => "image/bmp",
            ".webp" => "image/webp",
            ".svg" => "image/svg+xml",
            _ => "application/octet-stream"
        };
    }
}
