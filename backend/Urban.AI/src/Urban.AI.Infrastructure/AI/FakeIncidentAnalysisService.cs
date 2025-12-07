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
    private readonly Random _random;
    #endregion

    public FakeIncidentAnalysisService(ILogger<FakeIncidentAnalysisService> logger)
    {
        _logger = logger;
        _random = new Random();
    }

    public async Task<Result<IncidentAnalysis>> AnalyzeImageAsync(Stream imageStream, CancellationToken cancellationToken)
    {
        try
        {
            await Task.Delay(500, cancellationToken);

            var categories = Enum.GetValues<IncidentCategory>();
            var severities = Enum.GetValues<IncidentSeverity>();

            var category = categories[_random.Next(categories.Length)];
            var severity = severities[_random.Next(severities.Length)];

            var (caption, description) = GenerateFakeAnalysis(category, severity);

            var analysis = IncidentAnalysis.Create(caption, description, category, severity);

            _logger.LogInformation(
                "Fake AI analysis completed: Category={Category}, Severity={Severity}",
                category,
                severity);

            return Result.Success(analysis);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during fake AI analysis");
            return Result.Failure<IncidentAnalysis>(IncidentErrors.AnalysisFailed);
        }
    }

    private (string Caption, string Description) GenerateFakeAnalysis(IncidentCategory category, IncidentSeverity severity)
    {
        var severityText = severity switch
        {
            IncidentSeverity.Low => "minor",
            IncidentSeverity.Medium => "moderate",
            IncidentSeverity.High => "severe",
            IncidentSeverity.Critical => "critical",
            _ => "unknown"
        };

        return category switch
        {
            IncidentCategory.Stairs => (
                "Stairs blocking accessibility",
                $"The image shows stairs without proper ramps or alternative access routes. This presents a {severityText} barrier for people with mobility limitations."
            ),
            IncidentCategory.BrokenSidewalk => (
                "Broken sidewalk surface",
                $"The sidewalk exhibits {severityText} damage including cracks, holes, or uneven surfaces that pose safety hazards for pedestrians."
            ),
            IncidentCategory.MissingRamp => (
                "Missing accessibility ramp",
                $"The location lacks a necessary accessibility ramp, creating a {severityText} barrier for wheelchair users and people with mobility devices."
            ),
            IncidentCategory.WaterLeak => (
                "Water leak detected",
                $"A {severityText} water leak is visible, potentially from broken pipes or infrastructure, causing water accumulation and possible property damage."
            ),
            IncidentCategory.LackOfElectricity => (
                "Electrical infrastructure issue",
                $"The area shows {severityText} problems with electrical infrastructure, including non-functional streetlights or damaged electrical equipment."
            ),
            IncidentCategory.Other => (
                "Urban infrastructure issue",
                $"The image depicts a {severityText} urban infrastructure problem that requires attention from local authorities."
            ),
            _ => (
                "Unidentified urban issue",
                "The image shows an urban infrastructure concern that needs further evaluation."
            )
        };
    }
}
