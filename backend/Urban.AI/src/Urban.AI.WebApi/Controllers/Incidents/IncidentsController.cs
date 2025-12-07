namespace Urban.AI.WebApi.Controllers.Incidents;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Categories.SeedCategories;
using Urban.AI.Application.Incidents.CreateIncident;
using Urban.AI.Application.Incidents.Dtos;
using Urban.AI.Application.Incidents.UpdateIncidentStatus;
using Urban.AI.Domain.Incidents;
using Urban.AI.Application.Incidents.GetAllIncidents;
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
    [HttpPatch("{incidentId:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateIncidentStatus(
        [FromRoute] Guid incidentId,
        [FromBody] UpdateIncidentStatusRequest request,
        CancellationToken cancellationToken)
    {
        if (!Enum.TryParse<IncidentStatus>(request.Status, true, out var status))
        {
            return BadRequest($"Invalid status value. Allowed values: {string.Join(", ", Enum.GetNames<IncidentStatus>())}");
        }

        if (status == IncidentStatus.Pending)
        {
            return BadRequest("Cannot set status to Pending. Use Accepted or Rejected.");
        }

        IncidentPriority? priority = null;
        if (status == IncidentStatus.Accepted)
        {
            if (string.IsNullOrWhiteSpace(request.Priority))
            {
                return BadRequest("Priority is required when accepting an incident.");
            }

            if (!Enum.TryParse<IncidentPriority>(request.Priority, true, out var parsedPriority))
            {
                return BadRequest($"Invalid priority value. Allowed values: {string.Join(", ", Enum.GetNames<IncidentPriority>())}");
            }

            priority = parsedPriority;
        }

        var command = new UpdateIncidentStatusCommand(incidentId, status, priority);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return NoContent();
    }

    [Authorize]
    [HttpPost("seed-categories")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SeedCategories(CancellationToken cancellationToken)
    {
        var command = new SeedCategoriesCommand();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(new { Message = "Categories seeded successfully" });
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<IncidentResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllIncidents(CancellationToken cancellationToken)
    {
        var query = new GetAllIncidentsQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }
}
