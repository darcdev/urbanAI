namespace Urban.AI.Application.Leaders.CreateLeader;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.Clock;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Email;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
using Urban.AI.Domain.Leaders;
using Urban.AI.Domain.Users;
#endregion

internal sealed class CreateLeaderHandler : ICommandHandler<CreateLeaderCommand, Guid>
{
    #region Constants
    private const string LeaderRoleName = "Leader";
    #endregion

    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly ILeaderRepository _leaderRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IIdentityProvider _identityProvider;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateLeaderHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        ILeaderRepository leaderRepository,
        IDepartmentRepository departmentRepository,
        IMunicipalityRepository municipalityRepository,
        IAuthenticationService authenticationService,
        IIdentityProvider identityProvider,
        IEmailService emailService,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _leaderRepository = leaderRepository;
        _departmentRepository = departmentRepository;
        _municipalityRepository = municipalityRepository;
        _authenticationService = authenticationService;
        _identityProvider = identityProvider;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(CreateLeaderCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<Guid>(LeaderErrors.EmailAlreadyExists(request.Request.Email));
        }

        var department = await _departmentRepository.GetByIdAsync(request.Request.DepartmentId, cancellationToken);
        if (department is null)
        {
            return Result.Failure<Guid>(LeaderErrors.DepartmentNotFound);
        }

        var municipality = await _municipalityRepository.GetByIdAsync(request.Request.MunicipalityId, cancellationToken);
        if (municipality is null)
        {
            return Result.Failure<Guid>(LeaderErrors.MunicipalityNotFound);
        }

        var user = User.Register(
            request.Request.Email,
            request.Request.FirstName,
            request.Request.LastName,
            _dateTimeProvider.UtcNow);

        var identityId = await _authenticationService.RegisterAsync(
            user,
            request.Request.Password,
            cancellationToken);

        if (string.IsNullOrEmpty(identityId))
        {
            return Result.Failure<Guid>(LeaderErrors.FailedToCreateUserInKeycloak);
        }

        user.SetIdentityId(identityId);
        _userRepository.Add(user);

        var assignRoleResult = await _identityProvider.AssignRolesToUserAsync(
            identityId,
            [LeaderRoleName],
            cancellationToken);

        if (assignRoleResult.IsFailure)
        {
            return Result.Failure<Guid>(assignRoleResult.Error);
        }

        var leader = Leader.Create(
            user.Id,
            request.Request.DepartmentId,
            request.Request.MunicipalityId,
            request.Request.Latitude,
            request.Request.Longitude);

        _leaderRepository.Add(leader);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var emailSent = await _emailService.SendLeaderCredentialsEmailAsync(
            request.Request.Email,
            request.Request.FirstName,
            request.Request.LastName,
            request.Request.Email,
            request.Request.Password,
            cancellationToken);

        if (!emailSent)
        {
            return Result.Failure<Guid>(LeaderErrors.FailedToSendEmail);
        }

        return Result.Success(leader.Id);
    }
}
