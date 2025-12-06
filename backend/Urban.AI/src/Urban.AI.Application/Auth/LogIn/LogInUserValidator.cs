namespace Urban.AI.Application.Auth.LogIn;

#region Usings
using Urban.AI.Domain.Users.Resources;
using FluentValidation;
#endregion

internal sealed class LogInUserCommandValidator : AbstractValidator<LogInUserCommand>
{
    #region Constants
    private const int MinPasswordLength = 5;
    #endregion

    public LogInUserCommandValidator()
    {
        RuleFor(c => c.UserToLogIn.Email)
            .NotEmpty()
            .WithMessage(UserResources.EmailRequired)
            .EmailAddress()
            .WithMessage(UserResources.EmailInvalid);

        RuleFor(c => c.UserToLogIn.Password)
            .NotEmpty()
            .WithMessage(UserResources.PasswordRequired)
            .MinimumLength(MinPasswordLength)
            .WithMessage(string.Format(UserResources.PasswordMinLength, MinPasswordLength));
    }
}