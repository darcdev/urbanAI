namespace Urban.AI.Application.Organizations.GetOrganizations;

#region Usings
using FluentValidation;
#endregion

internal sealed class GetOrganizationsQueryInputValidation : AbstractValidator<GetOrganizationsQuery>
{
    #region Constants
    private const int MinPageNumber = 1;
    private const int MinPageSize = 1;
    private const int MaxPageSize = 100;
    #endregion

    public GetOrganizationsQueryInputValidation()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(MinPageNumber)
            .WithMessage($"Page number must be at least {MinPageNumber}");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(MinPageSize)
            .WithMessage($"Page size must be at least {MinPageSize}")
            .LessThanOrEqualTo(MaxPageSize)
            .WithMessage($"Page size must not exceed {MaxPageSize}");
    }
}
