namespace Urban.AI.Application.UserManagement.Users.UploadProfilePicture;

#region Usings
using Urban.AI.Application.Common.Extensions;
using FluentValidation;
#endregion

internal sealed class UploadProfilePictureValidation : AbstractValidator<UploadProfilePictureCommand>
{
    #region Constants
    private const int MaxFileSizeInMb = 2;
    private const long MaxFileSizeInBytes = MaxFileSizeInMb * 1024 * 1024;
    #endregion

    public UploadProfilePictureValidation()
    {
        RuleFor(x => x.Base64Image)
            .NotEmpty()
            .WithMessage("Profile picture is required")
            .Must(BeValidBase64)
            .WithMessage("Profile picture must be a valid Base64 string")
            .Must(BeValidImageFormat)
            .WithMessage("Profile picture must be a valid image format (JPEG, PNG, or WebP)")
            .Must(content => GetBase64Size(content) <= MaxFileSizeInBytes)
            .WithMessage($"Profile picture must not exceed {MaxFileSizeInMb} MB");
    }

    private static bool BeValidBase64(string base64String)
    {
        if (string.IsNullOrWhiteSpace(base64String))
            return false;

        var content = base64String.Contains("base64,")
            ? base64String.Split("base64,")[1]
            : base64String;

        try
        {
            Convert.FromBase64String(content);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool BeValidImageFormat(string base64String)
    {
        try
        {
            var content = base64String.Contains("base64,")
                ? base64String.Split("base64,")[1]
                : base64String;

            var imageBytes = Convert.FromBase64String(content);
            return imageBytes.IsValidImageFormat();
        }
        catch
        {
            return false;
        }
    }

    private static long GetBase64Size(string base64String)
    {
        var content = base64String.Contains("base64,")
            ? base64String.Split("base64,")[1]
            : base64String;

        return (long)(content.Length * 0.75);
    }
}