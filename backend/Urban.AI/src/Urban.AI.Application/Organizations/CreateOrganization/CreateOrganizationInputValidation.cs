namespace Urban.AI.Application.Organizations.CreateOrganization;

#region Usings
using FluentValidation;
using Urban.AI.Application.Organizations.Dtos;
using Urban.AI.Domain.Organizations.Resources;
using Urban.AI.Domain.Users;
#endregion

internal sealed class CreateOrganizationInputValidation : AbstractValidator<CreateOrganizationCommand>
{
    #region Constants
    private const int MinPasswordLength = 8;
    private const int MaxPasswordLength = 100;
    #endregion

    public CreateOrganizationInputValidation()
    {
        RuleFor(x => x.Request.FirstName)
            .NotEmpty()
            .MaximumLength(User.MaxFirstNameLength);

        RuleFor(x => x.Request.LastName)
            .NotEmpty()
            .MaximumLength(User.MaxLastNameLength);

        RuleFor(x => x.Request.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Request.Password)
            .NotEmpty()
            .MinimumLength(MinPasswordLength)
            .MaximumLength(MaxPasswordLength);

        RuleFor(x => x.Request.OrganizationName)
            .NotEmpty().WithMessage(OrganizationResources.InvalidOrganizationName)
            .MaximumLength(Domain.Organizations.Organization.MaxOrganizationNameLength);
    }
}
