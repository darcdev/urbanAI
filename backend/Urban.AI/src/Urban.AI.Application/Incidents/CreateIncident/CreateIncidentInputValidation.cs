namespace Urban.AI.Application.Incidents.CreateIncident;

#region Usings
using FluentValidation;
using Urban.AI.Domain.Incidents;
using Urban.AI.Domain.Incidents.Resources;
#endregion

internal sealed class CreateIncidentInputValidation : AbstractValidator<CreateIncidentCommand>
{
    #region Constants
    private const decimal MinLatitude = -90m;
    private const decimal MaxLatitude = 90m;
    private const decimal MinLongitude = -180m;
    private const decimal MaxLongitude = 180m;
    #endregion

    public CreateIncidentInputValidation()
    {
        RuleFor(x => x.Request.ImageStream)
            .NotNull()
            .WithMessage(IncidentResources.IncidentImageStreamRequired);

        RuleFor(x => x.Request.ImageFileName)
            .NotEmpty()
            .WithMessage(IncidentResources.IncidentImageFilenameRequired);

        RuleFor(x => x.Request.Latitude)
            .Must(lat => lat >= MinLatitude && lat <= MaxLatitude)
            .WithMessage(IncidentResources.IncidentInvalidLatitude);

        RuleFor(x => x.Request.Longitude)
            .Must(lon => lon >= MinLongitude && lon <= MaxLongitude)
            .WithMessage(IncidentResources.IncidentInvalidLongitude);

        When(x => !string.IsNullOrWhiteSpace(x.Request.CitizenEmail), () =>
        {
            RuleFor(x => x.Request.CitizenEmail)
                .EmailAddress()
                .WithMessage(IncidentResources.IncidentInvalidEmail);
        });

        RuleFor(x => x.Request.AdditionalComment)
            .MaximumLength(Incident.CommentMaxLength)
            .WithMessage(IncidentResources.IncidentCommentTooLong)
            .When(x => !string.IsNullOrWhiteSpace(x.Request.AdditionalComment));
    }
}
