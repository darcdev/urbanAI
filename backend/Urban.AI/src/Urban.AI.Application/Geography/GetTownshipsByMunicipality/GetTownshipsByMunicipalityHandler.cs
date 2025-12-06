namespace Urban.AI.Application.Geography.GetTownshipsByMunicipality;

#region Usings
using Urban.AI.Application.Common.Abstractions.CQRS;
using Urban.AI.Application.Geography.Dtos;
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Geography;
#endregion

internal sealed class GetTownshipsByMunicipalityHandler : IQueryHandler<GetTownshipsByMunicipalityQuery, IEnumerable<TownshipResponse>>
{
    private readonly ITownshipRepository _townshipRepository;
    private readonly IMunicipalityRepository _municipalityRepository;

    public GetTownshipsByMunicipalityHandler(
        ITownshipRepository townshipRepository,
        IMunicipalityRepository municipalityRepository)
    {
        _townshipRepository = townshipRepository;
        _municipalityRepository = municipalityRepository;
    }

    public async Task<Result<IEnumerable<TownshipResponse>>> Handle(
        GetTownshipsByMunicipalityQuery request,
        CancellationToken cancellationToken)
    {
        var municipality = await _municipalityRepository.GetByMunicipalityDaneCodeAsync(request.MunicipalityDaneCode, cancellationToken);
        if (municipality is null)
        {
            return Result.Failure<IEnumerable<TownshipResponse>>(GeographyErrors.MunicipalityNotFound);
        }

        var townships = await _townshipRepository.GetByMunicipalityDaneCodeAsync(request.MunicipalityDaneCode, cancellationToken);

        var response = townships.ToDto().ToList();

        return response;
    }
}