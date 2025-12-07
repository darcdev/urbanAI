namespace Urban.AI.Application.Geography;

#region Usings
using Urban.AI.Application.Geography.Dtos;
using Urban.AI.Domain.Geography; 
#endregion

internal static class GeographyExtensions
{
    #region Department Extensions  
    public static DepartmentResponse ToDto(this Department department)
    {
        return new DepartmentResponse(
            department.Id,
            department.DepartmentDaneCode,
            department.Name,
            department.Latitude,
            department.Longitude);
    }

    public static IEnumerable<DepartmentResponse> ToDto(this IEnumerable<Department> departments)
    {
        return departments.Select(d => d.ToDto());
    }
    #endregion

    #region Municipality Extensions
    public static MunicipalityResponse ToDto(this Municipality municipality)
    {
        return new MunicipalityResponse(
            municipality.Id,
            municipality.MunicipalityDaneCode,
            municipality.Name,
            municipality.DepartmentDaneCode,
            municipality.Latitude,
            municipality.Longitude);
    }

    public static IEnumerable<MunicipalityResponse> ToDto(this IEnumerable<Municipality> municipalities)
    {
        return municipalities.Select(m => m.ToDto());
    }
    #endregion

    #region Township Extensions  
    public static TownshipResponse ToDto(this Township township)
    {
        return new TownshipResponse(
            township.Id,
            township.TownshipDaneCode,
            township.Name,
            township.MunicipalityDaneCode);
    }

    public static IEnumerable<TownshipResponse> ToDto(this IEnumerable<Township> townships)
    {
        return townships.Select(t => t.ToDto());
    }
    #endregion
}