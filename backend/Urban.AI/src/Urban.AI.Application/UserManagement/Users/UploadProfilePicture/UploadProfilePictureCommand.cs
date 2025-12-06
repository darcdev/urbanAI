namespace Urban.AI.Application.UserManagement.Users.UploadProfilePicture;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
#endregion

public sealed record UploadProfilePictureCommand(string Base64Image) : ICommand<string>;