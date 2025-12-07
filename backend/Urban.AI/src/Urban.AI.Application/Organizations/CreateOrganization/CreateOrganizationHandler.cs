namespace Urban.AI.Application.Organizations.CreateOrganization;

#region Usings
using Urban.AI.Application.Common.Abstractions.Authentication;
using Urban.AI.Application.Common.Abstractions.Clock;
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Common.Abstractions.Email;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Organizations;
using Urban.AI.Domain.Users;
#endregion

internal sealed class CreateOrganizationHandler : ICommandHandler<CreateOrganizationCommand, Guid>
{
    #region Constants
    private const string OrganizationRoleName = "Organization";
    #endregion

    private readonly IUnitOfWork _unitOfWork;
    private readonly IUserRepository _userRepository;
    private readonly IOrganizationRepository _organizationRepository;
    private readonly IAuthenticationService _authenticationService;
    private readonly IIdentityProvider _identityProvider;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public CreateOrganizationHandler(
        IUnitOfWork unitOfWork,
        IUserRepository userRepository,
        IOrganizationRepository organizationRepository,
        IAuthenticationService authenticationService,
        IIdentityProvider identityProvider,
        IEmailService emailService,
        IDateTimeProvider dateTimeProvider)
    {
        _unitOfWork = unitOfWork;
        _userRepository = userRepository;
        _organizationRepository = organizationRepository;
        _authenticationService = authenticationService;
        _identityProvider = identityProvider;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Result<Guid>> Handle(CreateOrganizationCommand request, CancellationToken cancellationToken)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Request.Email, cancellationToken);
        if (existingUser is not null)
        {
            return Result.Failure<Guid>(OrganizationErrors.EmailAlreadyExists(request.Request.Email));
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
            return Result.Failure<Guid>(OrganizationErrors.FailedToCreateUserInKeycloak);
        }

        user.SetIdentityId(identityId);
        _userRepository.Add(user);

        var assignRoleResult = await _identityProvider.AssignRolesToUserAsync(
            identityId,
            [OrganizationRoleName],
            cancellationToken);

        if (assignRoleResult.IsFailure)
        {
            return Result.Failure<Guid>(assignRoleResult.Error);
        }

        var organization = Organization.Create(
            user.Id,
            request.Request.OrganizationName);

        _organizationRepository.Add(organization);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var emailSent = await _emailService.SendOrganizationCredentialsEmailAsync(
            request.Request.Email,
            request.Request.FirstName,
            request.Request.LastName,
            request.Request.Email,
            request.Request.OrganizationName,
            request.Request.Password,
            cancellationToken);

        if (!emailSent)
        {
            return Result.Failure<Guid>(OrganizationErrors.FailedToSendEmail);
        }

        return Result.Success(organization.Id);
    }
}
