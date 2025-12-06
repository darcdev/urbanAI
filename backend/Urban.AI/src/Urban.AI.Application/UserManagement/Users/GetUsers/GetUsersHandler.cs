namespace Urban.AI.Application.UserManagement.Users.GetUsers;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.UserManagement.Users.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class GetUsersHandler : IQueryHandler<GetUsersQuery, IEnumerable<UserResponse>>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityProvider _identityProvider;

    public GetUsersHandler(IUserRepository userRepository, IIdentityProvider identityProvider)
    {
        _userRepository = userRepository;
        _identityProvider = identityProvider;
    }

    public async Task<Result<IEnumerable<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        var usersResult = await _identityProvider.GetUsersAsync(cancellationToken);

        if (usersResult.IsFailure)
        {
            return Result.Failure<IEnumerable<UserResponse>>(usersResult.Error);
        }

        var users = await _userRepository.GetAllWithRolesAsync(cancellationToken);

        var userResponses = users.Select(user => user.ToUserResponse()).ToList();

        return Result.Success<IEnumerable<UserResponse>>(userResponses);
    }
}
