namespace Urban.AI.Application.Categories;

#region Usings
using Urban.AI.Application.Categories.Dtos;
using Urban.AI.Domain.Incidents;
#endregion

internal static class CategoryExtensions
{
    public static CategoryResponse ToResponse(this Category category)
    {
        return new CategoryResponse(
            category.Id,
            category.Code,
            category.Name,
            category.Description,
            category.Subcategories.Select(s => s.ToResponse()).ToList());
    }

    private static SubcategoryResponse ToResponse(this Subcategory subcategory)
    {
        return new SubcategoryResponse(
            subcategory.Id,
            subcategory.Name,
            subcategory.Description);
    }
}
