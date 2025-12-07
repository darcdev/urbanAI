namespace Urban.AI.Domain.Incidents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
#endregion

/// <summary>
/// Represents an incident subcategory
/// </summary>
public sealed class Subcategory : Entity
{
    #region Constants
    public const int NameMaxLength = 300;
    public const int DescriptionMaxLength = 500;
    #endregion

    private Subcategory(
        string name,
        string description,
        Guid categoryId) : base(Guid.NewGuid())
    {
        Name = name;
        Description = description;
        CategoryId = categoryId;
    }

    private Subcategory() { }

    public string Name { get; private set; }
    public string Description { get; private set; }
    public Guid CategoryId { get; private set; }

    public Category Category { get; private set; }

    public static Subcategory Create(string name, string description, Guid categoryId)
    {
        return new Subcategory(name, description, categoryId);
    }
}
