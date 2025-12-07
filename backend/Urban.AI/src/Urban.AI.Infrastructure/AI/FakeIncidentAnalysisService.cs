namespace Urban.AI.Infrastructure.AI;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
using Urban.AI.Domain.Incidents;
using Microsoft.Extensions.Logging;
#endregion

internal sealed class FakeIncidentAnalysisService : IIncidentAnalysisService
{
    #region Private Members
    private readonly ILogger<FakeIncidentAnalysisService> _logger;
    #endregion

    public FakeIncidentAnalysisService(ILogger<FakeIncidentAnalysisService> logger)
    {
        _logger = logger;
    }

    public async Task<Result<IncidentAnalysis>> AnalyzeImageAsync(Stream imageStream, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(500, cancellationToken);

            var description = "Fake AI analysis: Urban infrastructure issue detected. This is a simulated response for development purposes.";

            var analysis = IncidentAnalysis.CreateNotApplicable(description);

            _logger.LogInformation("Fake AI analysis completed with Not Applicable category");

            return Result.Success(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during fake AI analysis");
            return Result.Failure<IncidentAnalysis>(IncidentErrors.AnalysisFailed);
        }
    }
}
