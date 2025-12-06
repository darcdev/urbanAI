namespace Urban.AI.Application.Auth.Register;

#region Usings
using Urban.AI.Domain.Users.Resources;
using FluentValidation;
#endregion

public sealed class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    #region Constants
    private const int MinPasswordLength = 5;
    #endregion

    public RegisterUserValidator()
    {
        RuleFor(c => c.UserToRegister.Email)
            .NotEmpty()
            .WithMessage(UserResources.EmailRequired)
            .EmailAddress()
            .WithMessage(UserResources.EmailInvalid);

        RuleFor(c => c.UserToRegister.FirstName)
            .NotEmpty()
            .WithMessage(UserResources.FirstNameRequired);

        RuleFor(c => c.UserToRegister.LastName)
            .NotEmpty()
            .WithMessage(UserResources.LastNameRequired);

        RuleFor(c => c.UserToRegister.Password)
            .NotEmpty()
            .WithMessage(UserResources.PasswordRequired)
            .MinimumLength(MinPasswordLength)
            .WithMessage(string.Format(UserResources.PasswordMinLength, MinPasswordLength));
    }
}