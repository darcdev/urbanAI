namespace Urban.AI.Application.Auth.LogIn;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class LogInUserHandler : ICommandHandler<LogInUserCommand, Dtos.AccessTokenResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtProvider;

    public LogInUserHandler(IUserRepository userRepository, IJwtService jwtProvider)
    {
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task<Result<Dtos.AccessTokenResponse>> Handle(LogInUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailAsync(request.UserToLogIn.Email, cancellationToken);
        if (user is null) return Result.Failure<Dtos.AccessTokenResponse>(UserErrors.NotFound);

        var result = await _jwtProvider.GetAccessTokenAsync(request.UserToLogIn.Email, request.UserToLogIn.Password, cancellationToken);
        if (result.IsFailure) return Result.Failure<Dtos.AccessTokenResponse>(UserErrors.InvalidCredentials);

        return Result.Success(new Dtos.AccessTokenResponse(result.Value));
    }
}