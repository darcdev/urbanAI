namespace Urban.AI.Domain.Incidents;

public sealed class IncidentAnalysis
{
    public string Caption { get; private set; }
    public string Description { get; private set; }
    public IncidentCategory Category { get; private set; }
    public IncidentSeverity Severity { get; private set; }

    private IncidentAnalysis(
        string caption,
        string description,
        IncidentCategory category,
        IncidentSeverity severity)
    {
        Caption = caption;
        Description = description;
        Category = category;
        Severity = severity;
    }

    public static IncidentAnalysis Create(
        string caption,
        string description,
        IncidentCategory category,
        IncidentSeverity severity)
    {
        return new IncidentAnalysis(caption, description, category, severity);
    }
}
