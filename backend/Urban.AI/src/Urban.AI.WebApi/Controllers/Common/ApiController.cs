namespace Urban.AI.WebApi.Controllers.Common;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Microsoft.AspNetCore.Mvc;
#endregion

public abstract class ApiController : ControllerBase
{
    #region Constants
    private const string BusinessErrorType = "BusinessError";
    private const string Iso8601DateTimeFormat = "O"; // Round-trip date/time pattern (ISO 8601)
    #endregion

    protected IActionResult HandleFailure(Result result) =>
        CreateProblemDetails(
            result.Error.Code,
            result.Error.Name,
            StatusCodes.Status400BadRequest
        );

    protected IActionResult HandleFailure(Result result, int statusCode) =>
        CreateProblemDetails(
            result.Error.Code,
            result.Error.Name,
            statusCode
        );

    protected IActionResult HandleFailure(Error error) =>
        CreateProblemDetails(
            error.Code,
            error.Name,
            StatusCodes.Status400BadRequest
        );

    protected IActionResult HandleFailure(Error error, int statusCode) =>
        CreateProblemDetails(
            error.Code,
            error.Name,
            statusCode
        );

    #region Private methods
    private ObjectResult CreateProblemDetails(string title, string detail, int statusCode)
    {
        var problemDetails = new ProblemDetails
        {
            Type = BusinessErrorType,
            Title = title,
            Detail = detail,
            Status = statusCode,
            Instance = HttpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = HttpContext.TraceIdentifier;
        problemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString(Iso8601DateTimeFormat);

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }
    #endregion
}
