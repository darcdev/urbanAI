namespace Urban.AI.Application.UserManagement.Users.GetUserById;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class GetUserByIdHandler : IQueryHandler<GetUserByIdQuery, Dtos.UserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserByIdHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<Result<Dtos.UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdWithRolesAsync(request.UserId, cancellationToken);

        if (user is null)
        {
            return Result.Failure<Dtos.UserResponse>(UserErrors.NotFound);
        }

        return Result.Success(user.ToUserResponse());
    }
}
