namespace Urban.AI.Application.UserManagement.Roles.UpdateRole;

#region Usings
using Urban.AI.Domain.Users;
using Urban.AI.Domain.Users.Resources;
using FluentValidation;
#endregion

internal sealed class UpdateRoleInputValidation : AbstractValidator<UpdateRoleCommand>
{
    public UpdateRoleInputValidation()
    {
        RuleFor(x => x.RoleName)
            .NotEmpty()
            .WithMessage(RoleResources.RoleNameRequired);

        RuleFor(x => x.RoleToUpdate.Description)
            .MaximumLength(RoleConstants.MaxDescriptionLength)
            .WithMessage(string.Format(RoleResources.RoleDescriptionMaxLength, RoleConstants.MaxDescriptionLength));
    }
}
