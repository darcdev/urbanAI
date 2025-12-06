namespace Urban.AI.WebApi.Controllers.UserManagement;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.UserManagement.Roles.CreateRole;
using Urban.AI.Application.UserManagement.Roles.DeleteRole;
using Urban.AI.Application.UserManagement.Roles.Dtos;
using Urban.AI.Application.UserManagement.Roles.GetRoleById;
using Urban.AI.Application.UserManagement.Roles.GetRoles;
using Urban.AI.Application.UserManagement.Roles.UpdateRole;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/roles")]
public class RolesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetRoles(CancellationToken cancellationToken)
    {
        var query = new GetRolesQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("{roleName}")]
    public async Task<IActionResult> GetRoleByName(string roleName, CancellationToken cancellationToken)
    {
        var query = new GetRoleByIdQuery(roleName);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> CreateRole(CreateRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new CreateRoleCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Created();
    }

    [HttpPut("{roleName}")]
    public async Task<IActionResult> UpdateRole(string roleName, UpdateRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateRoleCommand(roleName, request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> DeleteRole(string roleName, CancellationToken cancellationToken)
    {
        var command = new DeleteRoleCommand(roleName);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
