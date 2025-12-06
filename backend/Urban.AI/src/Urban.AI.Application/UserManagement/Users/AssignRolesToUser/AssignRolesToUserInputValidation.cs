namespace Urban.AI.Application.UserManagement.Users.AssignRolesToUser;

#region Usings
using Urban.AI.Domain.Users.Resources;
using FluentValidation;
#endregion

internal sealed class AssignRolesToUserInputValidation : AbstractValidator<AssignRolesToUserCommand>
{
    public AssignRolesToUserInputValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(UserResources.UserIdRequired);

        RuleFor(x => x.RolesToAssign.RoleNames)
            .NotEmpty()
            .WithMessage(UserResources.AtLeastOneRoleRequired);
    }
}
