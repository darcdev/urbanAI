namespace Urban.AI.Domain.Incidents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
#endregion

/// <summary>
/// Represents an incident category
/// </summary>
public sealed class Category : Entity
{
    #region Constants
    public const int CodeMaxLength = 10;
    public const int NameMaxLength = 200;
    public const int DescriptionMaxLength = 500;
    #endregion

    private readonly List<Subcategory> _subcategories = new();

    private Category(
        string code,
        string name,
        string description) : base(Guid.NewGuid())
    {
        Code = code;
        Name = name;
        Description = description;
    }

    private Category() { }

    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    public IReadOnlyCollection<Subcategory> Subcategories => _subcategories.AsReadOnly();

    public static Category Create(string code, string name, string description)
    {
        return new Category(code, name, description);
    }

    public void AddSubcategory(Subcategory subcategory)
    {
        _subcategories.Add(subcategory);
    }
}
