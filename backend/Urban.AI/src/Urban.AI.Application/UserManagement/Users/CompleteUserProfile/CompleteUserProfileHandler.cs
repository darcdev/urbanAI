namespace Urban.AI.Application.UserManagement.Users.CompleteUserProfile;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Users;
#endregion

internal sealed class CompleteUserProfileHandler : ICommandHandler<CompleteUserProfileCommand>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IUserContext _userContext;

    public CompleteUserProfileHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IUserContext userContext)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _userContext = userContext;
    }

    public async Task<Result> Handle(CompleteUserProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = _userContext.UserId;

        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null) return Result.Failure(UserErrors.NotFound);
        if (user.UserDetails is not null) return Result.Failure(UserErrors.UserDetailsAlreadyCompleted);

        var personalInfo = request.UserDetails.ToDomainPersonalInfo();
        var contactInfo = request.UserDetails.ToDomainContactInfo();
        user.CompleteProfile(personalInfo, contactInfo);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
