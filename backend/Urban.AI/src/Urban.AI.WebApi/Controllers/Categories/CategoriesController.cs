namespace Urban.AI.WebApi.Controllers.Categories;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Categories.Dtos;
using Urban.AI.Application.Categories.GetCategories;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;
#endregion

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/categories")]
public class CategoriesController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories(CancellationToken cancellationToken)
    {
        var query = new GetCategoriesQuery();
        var result = await _sender.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result);
        }

        return Ok(result.Value);
    }
}
