namespace Urban.AI.Domain.Incidents;

using Urban.AI.Domain.Common.Abstractions;

public interface IIncidentAnalysisService
{
    Task<Result<IncidentAnalysis>> AnalyzeImageAsync(Stream imageStream, CancellationToken cancellationToken);
}
