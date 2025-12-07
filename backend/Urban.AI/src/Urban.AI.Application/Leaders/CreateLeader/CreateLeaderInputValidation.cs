namespace Urban.AI.Application.Leaders.CreateLeader;

#region Usings
using FluentValidation;
using Urban.AI.Application.Leaders.Dtos;
using Urban.AI.Domain.Leaders.Resources;
using Urban.AI.Domain.Users;
#endregion

internal sealed class CreateLeaderInputValidation : AbstractValidator<CreateLeaderCommand>
{
    #region Constants
    private const int MinPasswordLength = 8;
    private const int MaxPasswordLength = 100;
    private const decimal MinLatitude = -90m;
    private const decimal MaxLatitude = 90m;
    private const decimal MinLongitude = -180m;
    private const decimal MaxLongitude = 180m;
    #endregion

    public CreateLeaderInputValidation()
    {
        RuleFor(x => x.Request.FirstName)
            .NotEmpty().WithMessage(LeaderResources.UserNotFound)
            .MaximumLength(User.MaxFirstNameLength);

        RuleFor(x => x.Request.LastName)
            .NotEmpty().WithMessage(LeaderResources.UserNotFound)
            .MaximumLength(User.MaxLastNameLength);

        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(MinPasswordLength)
            .MaximumLength(MaxPasswordLength);

        RuleFor(x => x.Request.DepartmentId)
            .NotEmpty().WithMessage(LeaderResources.DepartmentNotFound);

        RuleFor(x => x.Request.MunicipalityId)
            .NotEmpty().WithMessage(LeaderResources.MunicipalityNotFound);

        RuleFor(x => x.Request.Latitude)
            .InclusiveBetween(MinLatitude, MaxLatitude)
            .WithMessage(LeaderResources.InvalidCoordinates);

        RuleFor(x => x.Request.Longitude)
            .InclusiveBetween(MinLongitude, MaxLongitude)
            .WithMessage(LeaderResources.InvalidCoordinates);
    }
}
