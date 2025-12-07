namespace Urban.AI.Application.Leaders.UpdateLeader;

#region Usings
using FluentValidation;
using Urban.AI.Domain.Users;
#endregion

internal sealed class UpdateLeaderCommandInputValidation : AbstractValidator<UpdateLeaderCommand>
{
    public UpdateLeaderCommandInputValidation()
    {
        RuleFor(x => x.LeaderId)
            .NotEmpty()
            .WithMessage("Leader ID is required");

        RuleFor(x => x.Request.FirstName)
            .NotEmpty()
            .WithMessage("First name is required")
            .MaximumLength(User.MaxFirstNameLength)
            .WithMessage($"First name must not exceed {User.MaxFirstNameLength} characters");

        RuleFor(x => x.Request.LastName)
            .NotEmpty()
            .WithMessage("Last name is required")
            .MaximumLength(User.MaxLastNameLength)
            .WithMessage($"Last name must not exceed {User.MaxLastNameLength} characters");
    }
}
