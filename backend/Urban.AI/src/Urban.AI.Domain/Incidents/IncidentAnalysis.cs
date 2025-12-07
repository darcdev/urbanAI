namespace Urban.AI.Domain.Incidents;

public sealed class IncidentAnalysis
{
    #region Constants
    public const int DescriptionMaxLength = 500;
    #endregion

    public Guid? CategoryId { get; private set; }
    public Guid? SubcategoryId { get; private set; }
    public string Description { get; private set; }

    private IncidentAnalysis(
        Guid? categoryId,
        Guid? subcategoryId,
        string description)
    {
        CategoryId = categoryId;
        SubcategoryId = subcategoryId;
        Description = description;
    }

    public static IncidentAnalysis Create(
        Guid? categoryId,
        Guid? subcategoryId,
        string description)
    {
        return new IncidentAnalysis(categoryId, subcategoryId, description);
    }

    public static IncidentAnalysis CreateNotApplicable(string description)
    {
        return new IncidentAnalysis(null, null, description);
    }
}
