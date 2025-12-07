namespace Urban.AI.Domain.Incidents;

#region Usings
using Urban.AI.Domain.Common.Abstractions;
#endregion

public static class CategoryErrors
{
    public static readonly Error NotFound = new(
        "Category.NotFound",
        "Category not found");

    public static readonly Error SubcategoryNotFound = new(
        "Category.SubcategoryNotFound",
        "Subcategory not found");

    public static readonly Error SubcategoryNotBelongsToCategory = new(
        "Category.SubcategoryNotBelongsToCategory",
        "The subcategory does not belong to the specified category");
}
