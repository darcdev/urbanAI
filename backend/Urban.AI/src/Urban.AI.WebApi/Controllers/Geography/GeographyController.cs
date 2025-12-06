namespace Urban.AI.WebApi.Controllers.Geography;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Geography.GetDepartments;
using Urban.AI.Application.Geography.GetMunicipalitiesByDepartment;
using Urban.AI.Application.Geography.GetTownshipsByMunicipality;
using Urban.AI.Application.Geography.SeedGeographyData;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[Authorize]
[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/geography")]
public class GeographyController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet("departments")]
    public async Task<IActionResult> GetDepartments(CancellationToken cancellationToken)
    {
        var query = new GetDepartmentsQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("municipalities/{departmentDaneCode}")]
    public async Task<IActionResult> GetMunicipalitiesByDepartment(
        string departmentDaneCode,
        CancellationToken cancellationToken)
    {
        var query = new GetMunicipalitiesByDepartmentQuery(departmentDaneCode);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpGet("townships/{municipalityDaneCode}")]
    public async Task<IActionResult> GetTownshipsByMunicipality(
        string municipalityDaneCode,
        CancellationToken cancellationToken)
    {
        var query = new GetTownshipsByMunicipalityQuery(municipalityDaneCode);
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }

    [HttpPost("seed")]
    public async Task<IActionResult> SeedGeographyData(CancellationToken cancellationToken)
    {
        var command = new SeedGeographyDataCommand();
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(new { Message = "Geography data seeded successfully" });
    }
}