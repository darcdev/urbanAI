namespace Urban.AI.WebApi.Controllers.Incidents;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Categories.SeedCategories;
using Urban.AI.Application.Incidents.CreateIncident;
using Urban.AI.Application.Incidents.Dtos;
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
}
