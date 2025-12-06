namespace Urban.AI.Application.UserManagement.Roles.CreateRole;

#region Usings
using Urban.AI.Domain.Users;
using Urban.AI.Domain.Users.Resources;
using FluentValidation;
#endregion

internal sealed class CreateRoleInputValidation : AbstractValidator<CreateRoleCommand>
{
    public CreateRoleInputValidation()
    {
        RuleFor(x => x.RoleToCreate.Name)
            .NotEmpty()
            .WithMessage(RoleResources.RoleNameRequired)
            .MaximumLength(RoleConstants.MaxNameLength)
            .WithMessage(string.Format(RoleResources.RoleNameMaxLength, RoleConstants.MaxNameLength));

        RuleFor(x => x.RoleToCreate.Description)
            .MaximumLength(RoleConstants.MaxDescriptionLength)
            .WithMessage(string.Format(RoleResources.RoleDescriptionMaxLength, RoleConstants.MaxDescriptionLength));
    }
}
