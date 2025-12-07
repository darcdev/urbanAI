namespace Urban.AI.WebApi.Controllers.Organizations;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Organizations.CreateOrganization;
using Urban.AI.Application.Organizations.Dtos;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

//[Authorize(Roles = "Admin")]
[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/organizations")]
public class OrganizationsController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateOrganization(
        [FromBody] CreateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateOrganizationCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
            nameof(CreateOrganization),
            new { id = result.Value },
            new { id = result.Value });
    }
}
