namespace Urban.AI.WebApi.Controllers.UserManagement;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.UserManagement.Users.AssignRolesToUser;
using Urban.AI.Application.UserManagement.Users.CompleteUserProfile;
using Urban.AI.Application.UserManagement.Users.DeleteUser;
using Urban.AI.Application.UserManagement.Users.Dtos;
using Urban.AI.Application.UserManagement.Users.GetUserById;
using Urban.AI.Application.UserManagement.Users.GetUsers;
using Urban.AI.Application.UserManagement.Users.UpdateUser;
using Urban.AI.Application.UserManagement.Users.UploadProfilePicture;
using Urban.AI.Domain.Users;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/users")]
public class UsersController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetUsers(CancellationToken cancellationToken)
    {
        var query = new GetUsersQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("{userId:guid}")]
    public async Task<IActionResult> GetUserById(Guid userId, CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(userId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code == UserErrors.NotFound.Code 
                ? HandleFailure(result.Error, StatusCodes.Status404NotFound)
                : HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpPut("{userId:guid}")]
    public async Task<IActionResult> UpdateUser(Guid userId, UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(userId, request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code == UserErrors.EmailAlreadyExists("").Code
                ? HandleFailure(result.Error, StatusCodes.Status409Conflict)
                : HandleFailure(result);
        }

        return NoContent();
    }

    [HttpDelete("{userId:guid}")]
    public async Task<IActionResult> DeleteUser(Guid userId, CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(userId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPost("{userId:guid}/roles")]
    public async Task<IActionResult> AssignRolesToUser(Guid userId, AssignRolesRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignRolesToUserCommand(userId, request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPost("complete-profile")]
    public async Task<IActionResult> CompleteUserProfile(CompleteUserProfileRequest request, CancellationToken cancellationToken)
    {
        var command = new CompleteUserProfileCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return result.Error.Code == UserErrors.UserDetailsAlreadyCompleted.Code
                ? HandleFailure(result.Error, StatusCodes.Status409Conflict)
                : HandleFailure(result);
        }

        return NoContent();
    }

    [HttpPost("profile-picture")]
    public async Task<IActionResult> UploadProfilePicture([FromBody] UploadProfilePictureRequest request, CancellationToken cancellationToken)
    {
        var command = new UploadProfilePictureCommand(request.Base64Image);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(new { ImageUrl = result.Value });
    }
}
