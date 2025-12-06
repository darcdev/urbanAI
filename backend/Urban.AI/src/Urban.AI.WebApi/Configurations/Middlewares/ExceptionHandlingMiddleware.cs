namespace Urban.AI.WebApi.Configurations.Middlewares;

#region Usings
using Microsoft.AspNetCore.Mvc;
using Urban.AI.Application.Common.Exceptions; 
#endregion

internal sealed class ExceptionHandlingMiddleware
{
    #region Constants
    private const string InputErrorType = "InputError";
    private const string ServerErrorType = "ServerError";
    private const string Iso8601DateTimeFormat = "O"; // Round-trip date/time pattern (ISO 8601)
    #endregion

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception exception)
        {
            var traceId = context.TraceIdentifier;

            _logger.LogError(
                exception,
                "Exception occurred: {Message} | TraceId: {TraceId}",
                exception.Message,
                traceId);

            var exceptionDetails = GetExceptionDetails(exception);

            var problemDetails = new ProblemDetails
            {
                Status = exceptionDetails.Status,
                Type = exceptionDetails.Type,
                Title = exceptionDetails.Title,
                Detail = exceptionDetails.Detail,
                Instance = context.Request.Path
            };

            problemDetails.Extensions["traceId"] = traceId;
            problemDetails.Extensions["timestamp"] = DateTime.UtcNow.ToString(Iso8601DateTimeFormat);

            if (exceptionDetails.Errors is not null)
            {
                problemDetails.Extensions["errors"] = exceptionDetails.Errors;
            }

            context.Response.StatusCode = exceptionDetails.Status;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }

    private static ExceptionDetails GetExceptionDetails(Exception exception)
    {
        return exception switch
        {
            ValidationException validationException => new ExceptionDetails(
                StatusCodes.Status400BadRequest,
                InputErrorType,
                "Validation error",
                "One or more validation errors has occurred",
                validationException.Errors),
            _ => new ExceptionDetails(
                StatusCodes.Status500InternalServerError,
                ServerErrorType,
                "Server error",
                "An unexpected error has occurred",
                null)
        };
    }

    private sealed record ExceptionDetails(
        int Status,
        string Type,
        string Title,
        string Detail,
        IEnumerable<object>? Errors);
}