namespace Urban.AI.Application.Geography.GetTownshipsByMunicipality;

using Urban.AI.Application.Geography.Dtos;
using Urban.AI.Application.Common.Abstractions.CQRS;

public record GetTownshipsByMunicipalityQuery(
    string MunicipalityDaneCode) : IQuery<IEnumerable<TownshipResponse>>;