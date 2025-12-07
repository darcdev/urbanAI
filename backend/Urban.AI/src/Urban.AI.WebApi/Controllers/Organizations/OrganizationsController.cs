namespace Urban.AI.WebApi.Controllers.Organizations;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Organizations.CreateOrganization;
using Urban.AI.Application.Organizations.GetOrganizations;
using Urban.AI.Application.Organizations.UpdateOrganization;
using Urban.AI.Application.Organizations.Dtos;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[Authorize(Roles = "Admin")]
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

    [HttpGet]
    public async Task<IActionResult> GetOrganizations(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetOrganizationsQuery(pageNumber, pageSize);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateOrganization(
        [FromRoute] Guid id,
        [FromBody] UpdateOrganizationRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateOrganizationCommand(id, request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
