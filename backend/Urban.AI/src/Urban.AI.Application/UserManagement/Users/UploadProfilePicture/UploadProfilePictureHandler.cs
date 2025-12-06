namespace Urban.AI.Application.UserManagement.Users.UploadProfilePicture;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Storage;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Common.File;
using Urban.AI.Domain.Users;
#endregion

internal sealed class UploadProfilePictureHandler : ICommandHandler<UploadProfilePictureCommand, string>
{
    #region Constants
    private const string UserAvatarsBucket = "user-avatars";
    private const string ImageExtension = ".jpg";
    #endregion

    #region Private Members
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;
    private readonly IStorageService _storageService;
    #endregion

    public UploadProfilePictureHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserContext userContext,
        IStorageService storageService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _userContext = userContext;
        _storageService = storageService;
    }

    public async Task<Result<string>> Handle(UploadProfilePictureCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null) return Result.Failure<string>(UserErrors.NotFound);
        if (user.UserDetails is null) return Result.Failure<string>(UserErrors.UserDetailsNotCompleted);

        var currentPictureUrl = user.UserDetails.ContactInfo.PictureUrl;
        if (!string.IsNullOrEmpty(currentPictureUrl))
        {
            await DeletePreviousProfilePicture(currentPictureUrl, cancellationToken);
        }

        var filename = $"{userId}{ImageExtension}";
        var path = $"profiles/{userId}";

        var file = File.CreateForSave(
            filename,
            request.Base64Image,
            path,
            UserAvatarsBucket,
            "image/jpeg");

        var saveResult = await _storageService.SaveFile(file, cancellationToken);
        if (saveResult.IsFailure) return Result.Failure<string>(saveResult.Error);

        var getPresignedUrlFile = File.CreateForGet(filename, path, UserAvatarsBucket);
        var presignedUrlResult = await _storageService.GetPresignedUrl(getPresignedUrlFile, cancellationToken: cancellationToken);
        if (presignedUrlResult.IsFailure) return Result.Failure<string>(presignedUrlResult.Error);

        user.UpdateProfilePicture(presignedUrlResult.Value);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success(presignedUrlResult.Value);
    }

    private async Task DeletePreviousProfilePicture(string pictureUrl, CancellationToken cancellationToken)
    {
        try
        {
            var uri = new Uri(pictureUrl);
            var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (segments.Length >= 2)
            {
                var filename = segments[^1];
                var path = string.Join("/", segments.Take(segments.Length - 1));

                var fileToDelete = File.CreateForDelete(path, UserAvatarsBucket);
                await _storageService.DeleteFile(fileToDelete, cancellationToken);
            }
        }
        catch
        {
        }
    }
}