namespace Urban.AI.WebApi.Controllers.Incidents;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Incidents.AcceptIncident;
using Urban.AI.Application.Incidents.CreateIncident;
using Urban.AI.Application.Incidents.Dtos;
using Urban.AI.Application.Incidents.RejectIncident;
using Urban.AI.Domain.Incidents;
using Urban.AI.WebApi.Controllers.Common;
using Urban.AI.WebApi.Controllers.Incidents.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/incidents")]
public class IncidentsController(ISender sender) : ApiController
{
    #region Constants
    private const int MaxImageSizeMb = 10;
    private const int MaxImageSizeBytes = MaxImageSizeMb * 1024 * 1024;
    private static readonly string[] AllowedImageExtensions = [".jpg", ".jpeg", ".png"];
    #endregion

    private readonly ISender _sender = sender;

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(IncidentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateIncident(
        [FromForm] CreateIncidentWebRequest webRequest,
        CancellationToken cancellationToken)
    {
        if (webRequest.Image == null || webRequest.Image.Length == 0)
        {
            return BadRequest("Image is required");
        }

        if (webRequest.Image.Length > MaxImageSizeBytes)
        {
            return BadRequest($"Image size must not exceed {MaxImageSizeMb} MB");
        }

        var extension = Path.GetExtension(webRequest.Image.FileName).ToLowerInvariant();
        if (!AllowedImageExtensions.Contains(extension))
        {
            return BadRequest("Image must be in JPEG or PNG format");
        }

        var imageStream = webRequest.Image.OpenReadStream();
        
        var request = new CreateIncidentRequest(
            imageStream,
            webRequest.Image.FileName,
            webRequest.Image.ContentType,
            webRequest.Latitude,
            webRequest.Longitude,
            webRequest.CitizenEmail,
            webRequest.AdditionalComment);

        var command = new CreateIncidentCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Created(string.Empty, result.Value);
    }

    [Authorize(Roles = "Leader")]
    [HttpPatch("{incidentId:guid}/accept")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AcceptIncident(
        [FromRoute] Guid incidentId,
        [FromBody] AcceptIncidentRequest request,
        CancellationToken cancellationToken)
    {
        var command = new AcceptIncidentCommand(incidentId, request.Priority);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [Authorize(Roles = "Leader")]
    [HttpPatch("{incidentId:guid}/reject")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RejectIncident(
        [FromRoute] Guid incidentId,
        CancellationToken cancellationToken)
    {
        var command = new RejectIncidentCommand(incidentId);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }
}
