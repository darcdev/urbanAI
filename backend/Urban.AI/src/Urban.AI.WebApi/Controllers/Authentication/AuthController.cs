namespace Urban.AI.WebApi.Controllers.Authentication;

#region Usings
using Asp.Versioning;
using Urban.AI.Application.Auth.Dtos;
using Urban.AI.Application.Auth.LogIn;
using Urban.AI.Application.Auth.Register;
using Urban.AI.Application.Auth.WhoAmI;
using Urban.AI.WebApi.Controllers.Common;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
#endregion

[ApiController]
[ApiVersion(ApiVersions.V1)]
[Route("api/v{version:apiVersion}/auth")]
public class AuthController(ISender sender) : ApiController
{
    private readonly ISender _sender = sender;

    [HttpPost("login")]
    public async Task<IActionResult> LogIn(LogInRequest request, CancellationToken cancellationToken)
    {
        var command = new LogInUserCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error, StatusCodes.Status401Unauthorized);
        }

        return Ok(result.Value);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(request);
        var result = await _sender.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return HandleFailure(result.Error, StatusCodes.Status409Conflict);
        }

        return Ok(result.Value);
    }

    [Authorize]
    [HttpGet("whoami")]
    public async Task<IActionResult> WhoAmI(CancellationToken cancellationToken)
    {
        var query = new WhoAmIUserQuery();
        var result = await _sender.Send(query, cancellationToken);

        return Ok(result.Value);
    }
}