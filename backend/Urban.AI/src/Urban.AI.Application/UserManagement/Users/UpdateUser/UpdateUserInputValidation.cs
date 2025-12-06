namespace Urban.AI.Application.UserManagement.Users.UpdateUser;

#region Usings
using Urban.AI.Domain.Users;
using Urban.AI.Domain.Users.Resources;
using FluentValidation;
#endregion

internal sealed class UpdateUserInputValidation : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserInputValidation()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(UserResources.UserIdRequired);

        RuleFor(x => x.UserToUpdate.FirstName)
            .NotEmpty()
            .WithMessage(UserResources.FirstNameRequired)
            .MaximumLength(User.MaxFirstNameLength)
            .WithMessage(string.Format(UserResources.FirstNameMaxLength, User.MaxFirstNameLength));

        RuleFor(x => x.UserToUpdate.LastName)
            .NotEmpty()
            .WithMessage(UserResources.LastNameRequired)
            .MaximumLength(User.MaxLastNameLength)
            .WithMessage(string.Format(UserResources.LastNameMaxLength, User.MaxLastNameLength));

        RuleFor(x => x.UserToUpdate.Email)
            .NotEmpty()
            .WithMessage(UserResources.EmailRequired)
            .EmailAddress()
            .WithMessage(UserResources.EmailInvalid);
    }
}
