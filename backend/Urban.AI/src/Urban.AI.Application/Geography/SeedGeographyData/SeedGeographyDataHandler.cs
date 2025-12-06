namespace Urban.AI.Application.Geography.SeedGeographyData;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Geography.Common;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
#endregion

internal sealed class SeedGeographyDataHandler : ICommandHandler<SeedGeographyDataCommand>
{
    #region Private Members
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IMunicipalityRepository _municipalityRepository;
    private readonly ITownshipRepository _townshipRepository;
    private readonly IGeographyDataParser _geographyDataParser;
    private readonly IUnitOfWork _unitOfWork;
    #endregion

    public SeedGeographyDataHandler(
        IDepartmentRepository departmentRepository,
        IMunicipalityRepository municipalityRepository,
        ITownshipRepository townshipRepository,
        IGeographyDataParser geographyDataParser,
        IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _municipalityRepository = municipalityRepository;
        _townshipRepository = townshipRepository;
        _geographyDataParser = geographyDataParser;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(
        SeedGeographyDataCommand request,
        CancellationToken cancellationToken)
    {
        await SeedDepartments(cancellationToken);
        await SeedMunicipalities(cancellationToken);
        await SeedTownships(cancellationToken);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }

    private async Task SeedDepartments(CancellationToken cancellationToken)
    {
        var existingDepartments = await _departmentRepository.GetAllAsync(cancellationToken);
        if (existingDepartments.Any()) return;

        var departments = await _geographyDataParser.ParseDepartmentsAsync(cancellationToken);

        foreach (var department in departments)
        {
            _departmentRepository.Add(department);
        }
    }

    private async Task SeedMunicipalities(CancellationToken cancellationToken)
    {
        var existingMunicipalities = await _municipalityRepository.GetAllAsync(cancellationToken);
        var existingMunicipalityDaneCodes = existingMunicipalities.Select(m => m.MunicipalityDaneCode).ToHashSet();

        var municipalities = await _geographyDataParser.ParseMunicipalitiesAsync(cancellationToken);

        foreach (var municipality in municipalities)
        {
            if (!existingMunicipalityDaneCodes.Contains(municipality.MunicipalityDaneCode))
            {
                _municipalityRepository.Add(municipality);
            }
        }
    }

    private async Task SeedTownships(CancellationToken cancellationToken)
    {
        var existingTownships = await _townshipRepository.GetAllAsync(cancellationToken);
        var existingTownshipDaneCodes = existingTownships.Select(t => t.TownshipDaneCode).ToHashSet();

        var townships = await _geographyDataParser.ParseTownshipsAsync(cancellationToken);

        foreach (var township in townships)
        {
            if (!existingTownshipDaneCodes.Contains(township.TownshipDaneCode))
            {
                _townshipRepository.Add(township);
            }
        }
    }
}
