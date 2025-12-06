namespace Urban.AI.Domain.Geography;

using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography.Resources;

public static class GeographyErrors
{
    public static readonly Error DepartmentNotFound = new(
        nameof(GeographyResources.DepartmentNotFound),
        GeographyResources.DepartmentNotFound);

    public static readonly Error MunicipalityNotFound = new(
        nameof(GeographyResources.MunicipalityNotFound),
        GeographyResources.MunicipalityNotFound);

    public static readonly Error TownshipNotFound = new(
        nameof(GeographyResources.TownshipNotFound),
        GeographyResources.TownshipNotFound);

    public static readonly Error InvalidDepartmentDaneCode = new(
        nameof(GeographyResources.InvalidDepartmentDaneCode),
        GeographyResources.InvalidDepartmentDaneCode);

    public static readonly Error InvalidMunicipalityDaneCode = new(
        nameof(GeographyResources.InvalidMunicipalityDaneCode),
        GeographyResources.InvalidMunicipalityDaneCode);

    public static readonly Error InvalidTownshipDaneCode = new(
        nameof(GeographyResources.InvalidTownshipDaneCode),
        GeographyResources.InvalidTownshipDaneCode);
}