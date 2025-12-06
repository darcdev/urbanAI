namespace Urban.AI.Application.Auth.Register;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.Clock;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class RegisterUserHandler : ICommandHandler<RegisterUserCommand, Guid>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IAuthenticationService _authenticationService;

    public RegisterUserHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IDateTimeProvider dateTimeProvider,
        IAuthenticationService authenticationService)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _dateTimeProvider = dateTimeProvider;
        _authenticationService = authenticationService;
    }

    public async Task<Result<Guid>> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.UserToRegister.Email, cancellationToken);
        if (existingUser is not null) return Result.Failure<Guid>(UserErrors.EmailAlreadyExists(request.UserToRegister.Email));

        var user = User.Register(
            request.UserToRegister.Email,
            request.UserToRegister.FirstName,
            request.UserToRegister.LastName,
            _dateTimeProvider.UtcNow
        );

        var identityId = await _authenticationService.RegisterAsync(
            user,
            request.UserToRegister.Password,
            cancellationToken);

        user.SetIdentityId(identityId);

        _userRepository.Add(user);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}