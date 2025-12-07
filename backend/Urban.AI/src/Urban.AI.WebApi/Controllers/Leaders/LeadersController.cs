namespace Urban.AI.WebApi.Controllers.Leaders;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Leaders.CreateLeader;
using Urban.AI.Application.Leaders.GetLeaders;
using Urban.AI.Application.Leaders.UpdateLeader;
using Urban.AI.Application.Leaders.Dtos;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[Authorize(Roles = "Admin")]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/leaders")]
public class LeadersController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateLeader(
        [FromBody] CreateLeaderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new CreateLeaderCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return CreatedAtAction(
            nameof(CreateLeader),
            new { id = result.Value },
            new { id = result.Value });
    }

    [HttpGet]
    public async Task<IActionResult> GetLeaders(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] Guid? departmentId = null,
        [FromQuery] Guid? municipalityId = null,
        CancellationToken cancellationToken = default)
    {
        var query = new GetLeadersQuery(pageNumber, pageSize, departmentId, municipalityId);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateLeader(
        [FromRoute] Guid id,
        [FromBody] UpdateLeaderRequest request,
        CancellationToken cancellationToken)
    {
        var command = new UpdateLeaderCommand(id, request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
