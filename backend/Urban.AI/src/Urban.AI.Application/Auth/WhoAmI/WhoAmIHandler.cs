namespace Urban.AI.Application.Auth.WhoAmI;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class WhoAmIHandler : IQueryHandler<WhoAmIUserQuery, Dtos.User>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public WhoAmIHandler(
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result<Dtos.User>> Handle(
        WhoAmIUserQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByEmailWithDetailsAsync(_userContext.Email, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Dtos.User>(UserErrors.NotFound);
        }

        Dtos.User userDto = user.ToDto(_userContext.Roles);

        return userDto;
    }
}