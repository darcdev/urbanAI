namespace Urban.AI.Application.UserManagement.Users.CompleteUserProfile;

#region Usings
using Urban.AI.Domain.Users.Resources;
using Urban.AI.Domain.Users.ValueObjects;
using FluentValidation;
#endregion

internal sealed class CompleteUserProfileInputValidation : AbstractValidator<CompleteUserProfileCommand>
{
    public CompleteUserProfileInputValidation()
    {
        RuleFor(x => x.UserDetails.PersonalInfo.BirthDate)
            .NotEmpty()
            .WithMessage(UserResources.BirthDateRequired)
            .LessThan(DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage(UserResources.BirthDateMustBeInPast);

        RuleFor(x => x.UserDetails.PersonalInfo.Gender)
            .NotEmpty()
            .WithMessage(UserResources.GenderRequired);

        RuleFor(x => x.UserDetails.PersonalInfo.DocumentType)
            .NotEmpty()
            .WithMessage(UserResources.DocumentTypeRequired);

        RuleFor(x => x.UserDetails.PersonalInfo.DocumentNumber)
            .NotEmpty()
            .WithMessage(UserResources.DocumentNumberRequired)
            .MaximumLength(PersonalInfo.MaxDocumentNumberLength)
            .WithMessage(string.Format(UserResources.DocumentNumberMaxLength, PersonalInfo.MaxDocumentNumberLength));

        RuleFor(x => x.UserDetails.ContactInfo.PhoneNumber)
            .NotEmpty()
            .WithMessage(UserResources.PhoneNumberRequired)
            .Matches(PhoneNumber.PhoneNumberPattern)
            .WithMessage(UserResources.PhoneNumberInvalidFormat);

        RuleFor(x => x.UserDetails.ContactInfo.PictureUrl)
            .MaximumLength(ContactInfo.MaxPictureUrlLength)
            .WithMessage(string.Format(UserResources.PictureUrlMaxLength, ContactInfo.MaxPictureUrlLength));

        RuleFor(x => x.UserDetails.ContactInfo.Biography)
            .MaximumLength(ContactInfo.MaxBiographyLength)
            .WithMessage(string.Format(UserResources.BiographyMaxLength, ContactInfo.MaxBiographyLength));
    }
}
