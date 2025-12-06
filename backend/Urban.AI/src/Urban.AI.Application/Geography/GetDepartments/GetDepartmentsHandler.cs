namespace Urban.AI.Application.Geography.GetDepartments;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Geography.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
#endregion

internal sealed class GetDepartmentsHandler : IQueryHandler<GetDepartmentsQuery, IEnumerable<DepartmentResponse>>
{
    private readonly IDepartmentRepository _departmentRepository;

    public GetDepartmentsHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Result<IEnumerable<DepartmentResponse>>> Handle(
        GetDepartmentsQuery request,
        CancellationToken cancellationToken)
    {
        var departments = await _departmentRepository.GetAllAsync(cancellationToken);

        var response = departments
            .OrderBy(d => d.Name)
            .ToDto()
            .ToList();

        return response;
    }
}