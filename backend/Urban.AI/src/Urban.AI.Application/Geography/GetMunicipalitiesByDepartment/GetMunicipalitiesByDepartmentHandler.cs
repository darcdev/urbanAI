namespace Urban.AI.Application.Geography.GetMunicipalitiesByDepartment;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Geography.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
#endregion

internal sealed class GetMunicipalitiesByDepartmentHandler : IQueryHandler<GetMunicipalitiesByDepartmentQuery, IEnumerable<MunicipalityResponse>>
{
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly IDepartmentRepository _departmentRepository;

    public GetMunicipalitiesByDepartmentHandler(
        IMunicipalityRepository municipalityRepository,
        IDepartmentRepository departmentRepository)
    {
        _municipalityRepository = municipalityRepository;
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<IEnumerable<MunicipalityResponse>>> Handle(
        GetMunicipalitiesByDepartmentQuery request,
        CancellationToken cancellationToken)
    {
        var department = await _departmentRepository.GetByDepartmentDaneCodeAsync(request.DepartmentDaneCode, cancellationToken);
        if (department is null)
        {
            return Result.Failure<IEnumerable<MunicipalityResponse>>(GeographyErrors.DepartmentNotFound);
        }

        var municipalities = await _municipalityRepository.GetByDepartmentDaneCodeAsync(request.DepartmentDaneCode, cancellationToken);

        var response = municipalities.ToDto().ToList();

        return response;
    }
}