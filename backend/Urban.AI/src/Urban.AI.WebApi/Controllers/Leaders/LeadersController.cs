namespace Urban.AI.WebApi.Controllers.Leaders;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Leaders.CreateLeader;
using Urban.AI.Application.Leaders.Dtos;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

//[Authorize(Roles = "Admin")]
[Authorize()]
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
}
